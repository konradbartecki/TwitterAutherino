using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.Web.Http;
using Windows.Web.Http.Headers;
using TwitterAutherino.Model;
//using HttpClient = System.Net.Http.HttpClient;
using wss = Windows.Storage.Streams;
using HttpClient = Windows.Web.Http.HttpClient;
using HttpMethod = Windows.Web.Http.HttpMethod;
using HttpRequestMessage = Windows.Web.Http.HttpRequestMessage;

namespace TwitterAutherino
{
    public class TwitterAuth
    {
        //Step 0 (TwitterAuth object creation)
        //Consumer key and consumer secret
        //Consumer is the guy or organization who is making the app
        public Keypair ConsumerKeypair { get; private set; }
        //Step 1 (Invisible HTTP GET)
        //Request token is the first step of oauth downloaded by HTTP silently to user
        public Keypair RequestKeypair { get; private set; }
        //Step 2 (Visible WebView)
        //Request response token is the token and verifier intercepted by WebView after user clicks "authorize" in the WebView
        public Keypair RequestResponseKeypair { get; private set; }
        //Step 3 (Invisible HTTP GET)
        //Access token and access token secret
        //Access token is the 'login and password' is the user who is logging in using sign-in-with-twitter button
        public Keypair AccessKeypair { get; private set; }

        public User User { get; private set; }

        public TwitterAuth(string ConsumerKey, string ConsumerSecret)
        {
            this.ConsumerKeypair = new Keypair(ConsumerKey, ConsumerSecret);
        }

        public async Task<Keypair> GetRequestTokenAsync(string callback)
        {
            RequestTokenSignature signature = new RequestTokenSignature(
                new BasicSignature(this.ConsumerKeypair), callback);

            HttpClient httpClient = new HttpClient();
            string GetResponse = await httpClient.GetStringAsync(new Uri(signature.RequestUri));


            string request_token = null;
            string oauth_token_secret = null;
            string[] keyValPairs = GetResponse.Split('&');

            for (int i = 0; i < keyValPairs.Length; i++)
            {
                string[] splits = keyValPairs[i].Split('=');
                switch (splits[0])
                {
                    case "oauth_token":
                        request_token = splits[1];
                        break;
                    case "oauth_token_secret":
                        oauth_token_secret = splits[1];
                        break;
                }
            }
            this.RequestKeypair = new Keypair(request_token, oauth_token_secret);
            return this.RequestKeypair;
        }

        public Keypair CheckWebViewNagitationStartingEvent(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            if (!args.Uri.Query.Contains("oauth_verifier=")) return null;
            //Stop and hide the browser
            sender.Stop();
            sender.Visibility = Visibility.Collapsed;
            //Get the token and verifier from the Uri
            string query = args.Uri.Query;
            query = query.Substring(query.IndexOf("oauth_token", StringComparison.Ordinal));
            string request_token = null;
            string oauth_verifier = null;
            String[] keyValPairs = query.Split('&');

            for (int i = 0; i < keyValPairs.Length; i++)
            {
                String[] splits = keyValPairs[i].Split('=');
                switch (splits[0])
                {
                    case "oauth_token":
                        request_token = splits[1];
                        break;
                    case "oauth_verifier":
                        oauth_verifier = splits[1];
                        break;
                }
            }
            this.RequestResponseKeypair = new Keypair(request_token, oauth_verifier);
            return RequestResponseKeypair;
        }

        public async Task<Keypair> GetAccessTokenAsync()
        {
            AccessTokenSignature signature = new AccessTokenSignature(
                new BasicSignature(this.ConsumerKeypair), this.RequestResponseKeypair);

            HttpStringContent httpContent = new HttpStringContent("oauth_verifier=" + this.RequestResponseKeypair.SecretKey, wss.UnicodeEncoding.Utf8);
            httpContent.Headers.ContentType = HttpMediaTypeHeaderValue.Parse("application/x-www-form-urlencoded");
            string authorizationHeaderParams = "oauth_consumer_key=\"" + this.ConsumerKeypair.PublicKey + "\", oauth_nonce=\"" + signature.Nonce + "\", oauth_signature_method=\"HMAC-SHA1\", oauth_signature=\"" + Uri.EscapeDataString(signature.SignedSignature) + "\", oauth_timestamp=\"" + signature.Timestamp + "\", oauth_token=\"" + Uri.EscapeDataString(this.RequestResponseKeypair.PublicKey) + "\", oauth_version=\"1.0\"";

            HttpClient httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.Authorization = new HttpCredentialsHeaderValue("OAuth", authorizationHeaderParams);
            var httpResponseMessage = await httpClient.PostAsync(new Uri(signature.RequestUri), httpContent);
            string response = await httpResponseMessage.Content.ReadAsStringAsync();

            String[] Tokens = response.Split('&');
            string oauth_token_secret = null;
            string access_token = null;
            string screen_name = null;

            for (int i = 0; i < Tokens.Length; i++)
            {
                String[] splits = Tokens[i].Split('=');
                switch (splits[0])
                {
                    case "screen_name":
                        screen_name = splits[1];
                        break;
                    case "oauth_token":
                        access_token = splits[1];
                        break;
                    case "oauth_token_secret":
                        oauth_token_secret = splits[1];
                        break;
                }
            }
            this.AccessKeypair = new Keypair(access_token, oauth_token_secret);
            this.User = new User
            {
                AccessKeypair = this.AccessKeypair,
                ScreenName = screen_name
            };
            return AccessKeypair;
        }

        public async Task<string> GetUserDetailsAsync()
        {
            GeneralGetSignature signature = new GeneralGetSignature(
                new BasicSignature(this.ConsumerKeypair), this.AccessKeypair, "https://api.twitter.com/1.1/account/verify_credentials.json");

            string authorizationHeaderParams = "oauth_consumer_key=\"" + signature.ConsumerKeypair.PublicKey + "\", oauth_nonce=\"" + signature.Nonce + "\", oauth_signature=\"" + Uri.EscapeDataString(signature.SignedSignature) + "\", oauth_signature_method=\"HMAC-SHA1\", oauth_timestamp=\"" + signature.Timestamp + "\", oauth_token=\"" + Uri.EscapeDataString(signature.AccessKeypair.PublicKey) + "\", oauth_version=\"1.0\"";

            HttpClient httpClient = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, new Uri(signature.RequestUri));
            request.Headers.Authorization = new HttpCredentialsHeaderValue("OAuth", authorizationHeaderParams);
            var response = await httpClient.SendRequestAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            return content;
        }

    }




}
