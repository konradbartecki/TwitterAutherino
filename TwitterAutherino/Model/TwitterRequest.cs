using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TwitterAutherino.Model
{
    internal class TwitterRequest
    {
        public string RequestAddress { get; set; }
        public string CallbackAddress { get; set; }

        public Keypair ConsumerKeypair { get; set; }
        public Keypair AccessKeypair { get; set; }

        public string Nonce { get; set; }
        public string Timestamp { get; set; }

        public RequestType RequestType { get; set; }

        public string UnsignedParametrsString { get; set; }
        public string SignedParameters { get; set; }

        internal TwitterRequest(string callback, Keypair consumerKeypair)
        {
            this.RequestAddress = KnownApiUri.RequestToken;
            this.CallbackAddress = callback;
            this.ConsumerKeypair = consumerKeypair;
            Initialize();
        }

        internal TwitterRequest(Keypair consumerKeypair) : this(KnownApiUri.DefaultCallback, consumerKeypair)
        {
            
        }

        private void Initialize()
        {
            this.Nonce = Cryptography.GetNonce();
            this.Timestamp = Cryptography.GetTimeStamp();
            this.Sign();
        }


        //public bool IsSigned { get; private set; }

        //internal TwitterRequest(Uri requestUri, Uri callbackUri, Keypair consumerKeypair)
        //{
        //    this.RequestUri = requestUri;
        //    this.CallbackUri = callbackUri;
        //    this.ConsumerKeypair = consumerKeypair;
        //    this.RequestMethod = RequestMethod.GET;


        //}

        //internal TwitterRequest(Uri requestUri, Keypair consumerKeypair, RequestMethod method) : this(requestUri, consumerKeypair, null, method)
        //{
        //}

        //internal TwitterRequest(Uri requestUri, Keypair consumerKeypair, Keypair accessKeypair, RequestMethod method)
        //{
        //    this.RequestUri = requestUri;
        //    this.ConsumerKeypair = consumerKeypair;
        //    this.AccessKeypair = accessKeypair;
        //    this.RequestMethod = method;
        //    this.Nonce = Cryptography.GetNonce();
        //    this.Timestamp = Cryptography.GetTimeStamp();
        //    this.Sign();
        //    this.CreateHttpClient();
        //}

        private void Sign()
        {
            this.UnsignedParametrsString = Cryptography.GetBaseStringSignature(this);
            //this.SignedParameters = Cryptography.GetSignature(this);               
        }

        public string GetAuthorizationHeader()
        {
            string token;
            if (this.RequestType == RequestType.GeneralGet)
                token = this.AccessKeypair.PublicKey;
            else
                token = this.ConsumerKeypair.PublicKey;
            string authorizationHeaderParams = "oauth_consumer_key=\"" + this.ConsumerKeypair.PublicKey + "\", oauth_nonce=\"" + this.Nonce + "\", oauth_signature_method=\"HMAC-SHA1\", oauth_signature=\"" + Uri.EscapeDataString(this.SignedParameters) + "\", oauth_timestamp=\"" + this.Timestamp + "\", oauth_token=\"" + Uri.EscapeDataString(token) + "\", oauth_version=\"1.0\"";
            return authorizationHeaderParams;
        }

        //private string GetUnsignedParametersString()
        //{
        //    String SigBaseStringParams = "oauth_consumer_key=" + this.ConsumerKeypair.PublicKey;
        //    SigBaseStringParams += "&" + "oauth_nonce=" + this.Nonce;
        //    SigBaseStringParams += "&" + "oauth_signature_method=HMAC-SHA1";
        //    SigBaseStringParams += "&" + "oauth_timestamp=" + this.Timestamp;
        //    if(this.AccessKeypair == null)
        //        SigBaseStringParams += "&" + "oauth_token=" + this.ConsumerKeypair.SecretKey;
        //    else
        //        SigBaseStringParams += "&" + "oauth_token=" + this.AccessKeypair.SecretKey;
        //    SigBaseStringParams += "&" + "oauth_version=1.0";
        //    String SigBaseString = this.RequestMethod + "&";
        //    SigBaseString += Uri.EscapeDataString(this.RequestUri.ToString()) + "&" + Uri.EscapeDataString(SigBaseStringParams);
        //    return SigBaseString;
        //}

        //private void CreateHttpClient()
        //{
        //    this.httpclient = new HttpClient();
        //}
    }

    //internal enum RequestMethod
    //{
    //    GET,
    //    POST
    //}
    internal enum RequestType
    {
        GetRequestToken,
        GetAccessToken,
        GeneralGet,
    }
}
