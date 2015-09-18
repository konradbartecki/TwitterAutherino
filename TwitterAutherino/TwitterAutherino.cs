using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TwitterAutherino.Model;

namespace TwitterAutherino
{
    public class TwitterAutherino
    {
        public Keypair ConsumerKeypair { get; private set; }
        public Keypair AccessKeypair { get; private set; }

        public TwitterAutherino(string ConsumerKey, string ConsumerSecret)
        {
            this.ConsumerKeypair = new Keypair(ConsumerKey, ConsumerSecret);
        }

        public Task<string> GetRequestTokenAsync(string callback)
        {
            TwitterRequest request = new TwitterRequest(callback, this.ConsumerKeypair);
        }



        private Task<string> SendRequestAsync(TwitterRequest request)
        {
            HttpClient client = new HttpClient();

        }
    }




}
