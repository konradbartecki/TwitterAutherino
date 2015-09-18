using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Notifications;

namespace TwitterAutherino.Model
{
    internal class Keypair
    {
        public string ConsumerKey { get; set; }
        public string ConsumerSecret { get; set; }
        public string AccessToken { get; set; }
        public string AccessTokenSecret { get; set; }
        public bool IsAccessTokenAvailable => isAccessTokenAvailable();
        private bool isAccessTokenAvailable()
        {
            if (string.IsNullOrWhiteSpace(this.AccessToken) || string.IsNullOrWhiteSpace(this.AccessTokenSecret))
                return false;
            return true;
        }


        public Keypair(string consumerKey, string consumerSecret) : this(consumerKey, consumerSecret, null, null)
        {
        }

        public Keypair(string consumerKey, string consumerSecret, string accessToken, string accessTokenSecret)
        {
            this.ConsumerKey = consumerKey;
            this.ConsumerSecret = consumerSecret;
            this.AccessToken = accessToken;
            this.AccessTokenSecret = accessTokenSecret;
        }
    }
}
