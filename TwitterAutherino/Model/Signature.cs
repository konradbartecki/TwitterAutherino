using System;
namespace TwitterAutherino.Model
{
    public class BasicSignature
    {
        public Keypair ConsumerKeypair { get; set; }
        public string Nonce { get; set; }
        public string Timestamp { get; set; }

        public BasicSignature(Keypair consumerKeypair)
        {
            this.ConsumerKeypair = consumerKeypair;
            this.Nonce = Cryptography.GetNonce();
            this.Timestamp = Cryptography.GetTimeStamp();
        }

        public BasicSignature()
        {
            
        }
    }

    public class RequestTokenSignature : BasicSignature
    {
        public string Callback { get; set; }
        public SigningKey SigningKey { get; set; }

        private const string RequestTokenApiUri = "https://api.twitter.com/oauth/request_token";

        private string SignatureParameters;
        private string SignatureBaseString;
        private string SignedSignature;
        public string RequestUri { get; private set; }

        public RequestTokenSignature(BasicSignature b, string callback)
        {
            this.Timestamp = b.Timestamp;
            this.ConsumerKeypair = b.ConsumerKeypair;
            this.Nonce = b.Nonce;
            this.Callback = callback;
            this.SignatureParameters = GetSigBaseStringParams();
            this.SignatureBaseString = GetSignatureBaseString();
            SigningKey = new SigningKey(ConsumerKeypair, SignatureBaseString);
            this.SignedSignature = Cryptography.GetSignature(SigningKey);
            RequestUri = RequestTokenApiUri + "?" + SignatureParameters + "&oauth_signature=" + Uri.EscapeDataString(SignedSignature);
        }

        public RequestTokenSignature()
        {
            
        }

        private string GetSigBaseStringParams()
        {
            string SigBaseStringParams = "oauth_callback=" + Uri.EscapeDataString(this.Callback);
            SigBaseStringParams += "&" + "oauth_consumer_key=" + this.ConsumerKeypair.PublicKey;
            SigBaseStringParams += "&" + "oauth_nonce=" + this.Nonce;
            SigBaseStringParams += "&" + "oauth_signature_method=HMAC-SHA1";
            SigBaseStringParams += "&" + "oauth_timestamp=" + this.Timestamp;
            SigBaseStringParams += "&" + "oauth_version=1.0";
            return SigBaseStringParams;
        }

        private string GetSignatureBaseString()
        {
            string SigBaseString = "GET&";
            SigBaseString += Uri.EscapeDataString(RequestTokenApiUri) + "&" + Uri.EscapeDataString(SignatureParameters);
            return SigBaseString;
        }      
    }

    public class AccessTokenSignature : BasicSignature
    {
        private const string AccessTokenApiUri = "https://api.twitter.com/oauth/access_token";
        public SigningKey SigningKey { get; set; }
        public Keypair RequestResponseKeypair { get; set; }
        private string SignatureParameters;
        private string SignatureBaseString;
        public string SignedSignature { get; private set; }
        public string RequestUri { get; private set; }

        public AccessTokenSignature(BasicSignature b, Keypair RequestResponseToken)
        {
            this.Timestamp = b.Timestamp;
            this.ConsumerKeypair = b.ConsumerKeypair;
            this.Nonce = b.Nonce;
            this.RequestResponseKeypair = RequestResponseToken;
            this.SignatureParameters = GetSigBaseStringParams();
            this.SignatureBaseString = GetSignatureBaseString();
            SigningKey = new SigningKey(ConsumerKeypair, SignatureBaseString);
            this.SignedSignature = Cryptography.GetSignature(SigningKey);
            RequestUri = AccessTokenApiUri + "?" + SignatureParameters + "&oauth_signature=" + Uri.EscapeDataString(SignedSignature);
        }

        private string GetSigBaseStringParams()
        {
            String SigBaseStringParams = "oauth_consumer_key=" + this.ConsumerKeypair.PublicKey;
            SigBaseStringParams += "&" + "oauth_nonce=" + this.Nonce;
            SigBaseStringParams += "&" + "oauth_signature_method=HMAC-SHA1";
            SigBaseStringParams += "&" + "oauth_timestamp=" + this.Timestamp;
            SigBaseStringParams += "&" + "oauth_token=" + this.RequestResponseKeypair.PublicKey;
            SigBaseStringParams += "&" + "oauth_version=1.0";
            return SigBaseStringParams;

        }

        private string GetSignatureBaseString()
        {
            String SigBaseString = "POST&";
            SigBaseString += Uri.EscapeDataString(AccessTokenApiUri) + "&" + Uri.EscapeDataString(this.SignatureParameters);
            return SigBaseString;
        }
    }

    public class GeneralGetSignature : BasicSignature
    {
        public SigningKey SigningKey { get; set; }
        public Keypair AccessKeypair { get; set; }
        private string SignatureParameters;
        private string SignatureBaseString;
        public string SignedSignature { get; private set; }
        public string RequestUri { get; private set; }

        public GeneralGetSignature(BasicSignature b, Keypair UserAccessKeypair, string requestUri)
        {
            this.Timestamp = b.Timestamp;
            this.ConsumerKeypair = b.ConsumerKeypair;
            this.Nonce = b.Nonce;
            this.AccessKeypair = UserAccessKeypair;
            this.RequestUri = requestUri;
            this.SignatureParameters = GetSigBaseStringParams();
            this.SignatureBaseString = GetSignatureBaseString();
            SigningKey = new SigningKey(ConsumerKeypair, UserAccessKeypair, SignatureBaseString);
            this.SignedSignature = Cryptography.GetSignature(SigningKey);
        }

        private string GetSigBaseStringParams()
        {
            String SigBaseStringParams = "oauth_consumer_key=" + this.ConsumerKeypair.PublicKey;
            SigBaseStringParams += "&" + "oauth_nonce=" + this.Nonce;
            SigBaseStringParams += "&" + "oauth_signature_method=HMAC-SHA1";
            SigBaseStringParams += "&" + "oauth_timestamp=" + this.Timestamp;
            SigBaseStringParams += "&" + "oauth_token=" + this.AccessKeypair.PublicKey;
            SigBaseStringParams += "&" + "oauth_version=1.0";
            return SigBaseStringParams;
        }

        private string GetSignatureBaseString()
        {
            String SigBaseString = "GET&";
            SigBaseString += Uri.EscapeDataString(this.RequestUri) + "&" + Uri.EscapeDataString(this.SignatureParameters);
            return SigBaseString;
        }
    }
}
