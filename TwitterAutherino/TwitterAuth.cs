using System;
using System.Net;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.Web.Http;
using Windows.Web.Http.Headers;
using Newtonsoft.Json;
using TwitterAutherino.Model;
using TwitterAutherino.Model.Exceptions;
using TwitterAutherino.Model.TwitterApi;
//using HttpClient = System.Net.Http.HttpClient;
using wss = Windows.Storage.Streams;

namespace TwitterAutherino
{
    public class TwitterAuth
    {
        public TwitterAuth(string ConsumerKey, string ConsumerSecret, string AccessKey, string AccessSecret)
        {
            ConsumerKeypair = new Keypair(ConsumerKey, ConsumerSecret);
            if (!string.IsNullOrWhiteSpace(AccessKey) && !string.IsNullOrWhiteSpace(AccessSecret))
                AccessKeypair = new Keypair(AccessKey, AccessSecret);
            this.webView = new WebView();
            webView.NavigationStarting += WebView_NavigationStarting;
        }
        public TwitterAuth(string ConsumerKey, string ConsumerSecret) : this(ConsumerKey, ConsumerSecret, null, null)
        {
        }

        private void WebView_NavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            this.RequestResponseKeypair = CheckWebViewNagitationStartingEvent(sender, args);
        }

        //Step 0 (TwitterAuth object creation)
        //Consumer key and consumer secret
        //Consumer is the guy or organization who is making the client app
        public Keypair ConsumerKeypair { get; }
        //Step 1 (Invisible HTTP GET)
        //Request token is the first step of oauth downloaded by HTTP silently
        public Keypair RequestKeypair { get; private set; }
        //Step 2 (Visible WebView)
        //Request response token is the token and verifier intercepted by WebView after user clicks "authorize" in the WebView
        public Keypair RequestResponseKeypair { get; private set; }
        //Step 3 (Invisible HTTP GET)
        //Access token and access token secret
        //Access token is the 'login and password' is the user who is logging in using sign-in-with-twitter button
        public Keypair AccessKeypair { get; private set; }

        public SimpleUser User { get; private set; }

        private WebView webView;
        private Flyout flyout;

        public async Task<Keypair> GetRequestTokenAsync(string callback)
        {
            var signature = new RequestTokenSignature(
                new BasicSignature(ConsumerKeypair), callback);

            var httpClient = new HttpClient();
            try
            {
                string GetResponse = await httpClient.GetStringAsync(new Uri(signature.RequestUri));
                string request_token = null;
                string oauth_token_secret = null;
                var keyValPairs = GetResponse.Split('&');

                for (var i = 0; i < keyValPairs.Length; i++)
                {
                    var splits = keyValPairs[i].Split('=');
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
                RequestKeypair = new Keypair(request_token, oauth_token_secret);
                return RequestKeypair;
            }
            catch (Exception e)
            {
                if (e.Message.Contains("401"))
                    throw new OAuthUnauthorizedException(Strings.ExceptionUnauthorized);
                else
                    throw;
            }
            return null;
        }

        public Keypair CheckWebViewNagitationStartingEvent(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            if (!args.Uri.Query.Contains("oauth_verifier=")) return null;
            //Stop and hide the browser
            sender.Stop();
            flyout.Hide();
            sender.Visibility = Visibility.Collapsed;
            //Get the token and verifier from the Uri
            var query = args.Uri.Query;
            query = query.Substring(query.IndexOf("oauth_token", StringComparison.Ordinal));
            string request_token = null;
            string oauth_verifier = null;
            var keyValPairs = query.Split('&');

            for (var i = 0; i < keyValPairs.Length; i++)
            {
                var splits = keyValPairs[i].Split('=');
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
            RequestResponseKeypair = new Keypair(request_token, oauth_verifier);
            GotRequestResponseKeypair?.Invoke(this, EventArgs.Empty);
            return RequestResponseKeypair;
        }

        public async Task<SimpleUser> GetAccessTokenAsync()
        {
            var signature = new AccessTokenSignature(
                new BasicSignature(ConsumerKeypair), RequestResponseKeypair);

            var httpContent = new HttpStringContent("oauth_verifier=" + RequestResponseKeypair.SecretKey,
                wss.UnicodeEncoding.Utf8);
            httpContent.Headers.ContentType = HttpMediaTypeHeaderValue.Parse("application/x-www-form-urlencoded");

            var authorizationHeaderParams = "oauth_consumer_key=\"" + ConsumerKeypair.PublicKey +
                                            "\", oauth_nonce=\"" + signature.Nonce +
                                            "\", oauth_signature_method=\"HMAC-SHA1\", oauth_signature=\"" + Uri.EscapeDataString(signature.SignedSignature) +
                                            "\", oauth_timestamp=\"" + signature.Timestamp +
                                            "\", oauth_token=\"" + Uri.EscapeDataString(RequestResponseKeypair.PublicKey) +
                                            "\", oauth_version=\"1.0\"";

            var httpClient = new HttpClient();

            try
            {
                httpClient.DefaultRequestHeaders.Authorization = new HttpCredentialsHeaderValue("OAuth",
                        authorizationHeaderParams);
                var httpResponseMessage = await httpClient.PostAsync(new Uri(signature.RequestUri), httpContent);
                var response = await httpResponseMessage.Content.ReadAsStringAsync();

                var Tokens = response.Split('&');
                string oauth_token_secret = null;
                string access_token = null;
                string screen_name = null;

                for (var i = 0; i < Tokens.Length; i++)
                {
                    var splits = Tokens[i].Split('=');
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
                AccessKeypair = new Keypair(access_token, oauth_token_secret);
                User = new SimpleUser
                {
                    AccessKeypair = AccessKeypair,
                    ScreenName = screen_name
                };
                return User;
            }
            catch (Exception e)
            {
                if (e.Message.Contains("401"))
                    throw new OAuthUnauthorizedException(Strings.ExceptionUnauthorized);
                else
                    throw;
            }
        }

        public async Task<User> GetUserDetailsAsync()
        {
            if(AccessKeypair == null) throw new OAuthMissingAccessTokenException(Strings.MissingAccessToken);

            var signature = new GeneralGetSignature(new BasicSignature(ConsumerKeypair), AccessKeypair,
                "https://api.twitter.com/1.1/account/verify_credentials.json");

            var authorizationHeaderParams = "oauth_consumer_key=\"" + signature.ConsumerKeypair.PublicKey +
                                            "\", oauth_nonce=\"" + signature.Nonce +
                                            "\", oauth_signature=\"" + Uri.EscapeDataString(signature.SignedSignature) +
                                            "\", oauth_signature_method=\"HMAC-SHA1\", oauth_timestamp=\"" + signature.Timestamp +
                                            "\", oauth_token=\"" + Uri.EscapeDataString(signature.AccessKeypair.PublicKey) +
                                            "\", oauth_version=\"1.0\"";
            try
            {
                var httpClient = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Get, new Uri(signature.RequestUri));
                request.Headers.Authorization = new HttpCredentialsHeaderValue("OAuth", authorizationHeaderParams);
                var response = await httpClient.SendRequestAsync(request);
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<User>(content);
            }
            catch (Exception e)
            {
                if (e.Message.Contains("401"))
                    throw new OAuthUnauthorizedException(Strings.ExceptionUnauthorized);
                else
                    throw;
            }
        }

        public Uri GetWebViewUri()
        {
            return new Uri("https://api.twitter.com/oauth/authorize?oauth_token=" + this.RequestKeypair.PublicKey);
        }

        /// <summary>
        /// Show twitter login web dialog
        /// </summary>
        /// <param name="placementTarget">Control on which the webview should be shown</param>
        public void ShowWebDialogFlyout(FrameworkElement placementTarget)
        {
            webView.Navigate(GetWebViewUri());
            flyout = new Flyout();
            Grid grid = new Grid();
            grid.Children.Add(webView);
            flyout.Content = grid;
            flyout.Placement = FlyoutPlacementMode.Full;
            flyout.ShowAt(placementTarget);
        }

        public event EventHandler GotRequestResponseKeypair;

        protected virtual void OnGotRequestResponseKeypair()
        {
            GotRequestResponseKeypair?.Invoke(this, EventArgs.Empty);
        }
    }
}