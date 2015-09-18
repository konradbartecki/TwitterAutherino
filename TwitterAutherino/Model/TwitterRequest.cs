using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitterAutherino.Model
{
    internal class TwitterRequest
    {
        public Uri RequestUri { get; set; }
        public Keypair Keypair { get; set; }
        public string Nonce { get; set; }
        public string Timestamp { get; set; }
        public RequestMethod RequestMethod { get; set; }
        public string UnsignedParametrsString { get; set; }
        public string SignedParameters { get; set; }
        public bool IsSigned { get; private set; }
        
        internal TwitterRequest(Uri requestUri, Keypair keypair, RequestMethod method)
        {
            this.RequestUri = requestUri;
            this.Keypair = keypair;
            this.RequestMethod = method;
            this.Nonce = Cryptography.GetNonce();
            this.Timestamp = Cryptography.GetTimeStamp();
            this.Sign();
        }

        private void Sign()
        {
            this.UnsignedParametrsString = Cryptography.
        }

        private string GetUnsignedParametersString()
        {
            String SigBaseStringParams = "oauth_consumer_key=" + this.Keypair.ConsumerKey;
            SigBaseStringParams += "&" + "oauth_nonce=" + this.Nonce;
            SigBaseStringParams += "&" + "oauth_signature_method=HMAC-SHA1";
            SigBaseStringParams += "&" + "oauth_timestamp=" + this.Timestamp;
            SigBaseStringParams += "&" + "oauth_token=" + access_token;
            SigBaseStringParams += "&" + "oauth_version=1.0";
            String SigBaseString = "GET&";
        }
    }

    internal enum RequestMethod
    {
        GET,
        POST
    }

    internal enum RequestType
    {
        GetRequestToken,
        GetAccessToken,

    }
}
