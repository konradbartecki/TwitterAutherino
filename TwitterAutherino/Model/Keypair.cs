using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Notifications;

namespace TwitterAutherino.Model
{
    public class Keypair
    {
        public string PublicKey { get; set; }
        public string SecretKey { get; set; }

        public Keypair(string KeyOrToken, string secretToken)
        {
            this.PublicKey = KeyOrToken;
            this.SecretKey = secretToken;
        }
        //public string AccessToken { get; set; }
        //public string AccessTokenSecret { get; set; }
        //public bool IsAccessTokenAvailable => isAccessTokenAvailable();
        //private bool isAccessTokenAvailable()
        //{
        //    if (string.IsNullOrWhiteSpace(this.AccessToken) || string.IsNullOrWhiteSpace(this.AccessTokenSecret))
        //        return false;
        //    return true;
        //}


        //public Keypair(string consumerKey, string consumerSecret) : this(consumerKey, consumerSecret, null, null)
        //{
        //}

        //public Keypair(string consumerKey, string consumerSecret, string accessToken, string accessTokenSecret)
        //{
        //    this.ConsumerKey = consumerKey;
        //    this.ConsumerSecret = consumerSecret;
        //    this.AccessToken = accessToken;
        //    this.AccessTokenSecret = accessTokenSecret;
        }
    }
}
