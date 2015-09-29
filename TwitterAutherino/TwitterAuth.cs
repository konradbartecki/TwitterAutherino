using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TwitterAutherino.Model;

namespace TwitterAutherino
{
    public class TwitterAuth
    {
        public Keypair ConsumerKeypair { get; private set; }
        public Keypair AccessKeypair { get; private set; }

        public Keypair RequestResponseKeypair { get; private set; }


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

        public async Task<Keypair> GetAccessTokenAsync()
        {
            
        }

    }




}
