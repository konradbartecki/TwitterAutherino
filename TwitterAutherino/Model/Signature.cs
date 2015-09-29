using System;

namespace TwitterAutherino.Model
{
    public class BasicSignature
    {
        public BasicSignature(Keypair consumerKeypair)
        {
            ConsumerKeypair = consumerKeypair;
            Nonce = Cryptography.GetNonce();
            Timestamp = Cryptography.GetTimeStamp();
        }

        public BasicSignature()
        {
        }

        public Keypair ConsumerKeypair { get; set; }
        public string Nonce { get; set; }
        public string Timestamp { get; set; }
    }

    public class RequestTokenSignature : BasicSignature
    {
        private const string RequestTokenApiUri = "https://api.twitter.com/oauth/request_token";
        private readonly string SignatureBaseString;

        private readonly string SignatureParameters;
        private readonly string SignedSignature;

        public RequestTokenSignature(BasicSignature b, string callback)
        {
            Timestamp = b.Timestamp;
            ConsumerKeypair = b.ConsumerKeypair;
            Nonce = b.Nonce;
            Callback = callback;
            SignatureParameters = GetSigBaseStringParams();
            SignatureBaseString = GetSignatureBaseString();
            SigningKey = new SigningKey(ConsumerKeypair, SignatureBaseString);
            SignedSignature = Cryptography.GetSignature(SigningKey);
            RequestUri = RequestTokenApiUri + "?" + SignatureParameters + "&oauth_signature=" +
                         Uri.EscapeDataString(SignedSignature);
        }

        public RequestTokenSignature()
        {
        }

        public string Callback { get; set; }
        public SigningKey SigningKey { get; set; }
        public string RequestUri { get; private set; }

        private string GetSigBaseStringParams()
        {
            var SigBaseStringParams = "oauth_callback=" + Uri.EscapeDataString(Callback);
            SigBaseStringParams += "&" + "oauth_consumer_key=" + ConsumerKeypair.PublicKey;
            SigBaseStringParams += "&" + "oauth_nonce=" + Nonce;
            SigBaseStringParams += "&" + "oauth_signature_method=HMAC-SHA1";
            SigBaseStringParams += "&" + "oauth_timestamp=" + Timestamp;
            SigBaseStringParams += "&" + "oauth_version=1.0";
            return SigBaseStringParams;
        }

        private string GetSignatureBaseString()
        {
            var SigBaseString = "GET&";
            SigBaseString += Uri.EscapeDataString(RequestTokenApiUri) + "&" + Uri.EscapeDataString(SignatureParameters);
            return SigBaseString;
        }
    }

    public class AccessTokenSignature : BasicSignature
    {
        private const string AccessTokenApiUri = "https://api.twitter.com/oauth/access_token";
        private readonly string SignatureBaseString;
        private readonly string SignatureParameters;

        public AccessTokenSignature(BasicSignature b, Keypair RequestResponseToken)
        {
            Timestamp = b.Timestamp;
            ConsumerKeypair = b.ConsumerKeypair;
            Nonce = b.Nonce;
            RequestResponseKeypair = RequestResponseToken;
            SignatureParameters = GetSigBaseStringParams();
            SignatureBaseString = GetSignatureBaseString();
            SigningKey = new SigningKey(ConsumerKeypair, SignatureBaseString);
            SignedSignature = Cryptography.GetSignature(SigningKey);
            RequestUri = AccessTokenApiUri + "?" + SignatureParameters + "&oauth_signature=" +
                         Uri.EscapeDataString(SignedSignature);
        }

        public SigningKey SigningKey { get; set; }
        public Keypair RequestResponseKeypair { get; set; }
        public string SignedSignature { get; }
        public string RequestUri { get; private set; }

        private string GetSigBaseStringParams()
        {
            var SigBaseStringParams = "oauth_consumer_key=" + ConsumerKeypair.PublicKey;
            SigBaseStringParams += "&" + "oauth_nonce=" + Nonce;
            SigBaseStringParams += "&" + "oauth_signature_method=HMAC-SHA1";
            SigBaseStringParams += "&" + "oauth_timestamp=" + Timestamp;
            SigBaseStringParams += "&" + "oauth_token=" + RequestResponseKeypair.PublicKey;
            SigBaseStringParams += "&" + "oauth_version=1.0";
            return SigBaseStringParams;
        }

        private string GetSignatureBaseString()
        {
            var SigBaseString = "POST&";
            SigBaseString += Uri.EscapeDataString(AccessTokenApiUri) + "&" + Uri.EscapeDataString(SignatureParameters);
            return SigBaseString;
        }
    }

    public class GeneralGetSignature : BasicSignature
    {
        private readonly string SignatureBaseString;
        private readonly string SignatureParameters;

        public GeneralGetSignature(BasicSignature b, Keypair UserAccessKeypair, string requestUri)
        {
            Timestamp = b.Timestamp;
            ConsumerKeypair = b.ConsumerKeypair;
            Nonce = b.Nonce;
            AccessKeypair = UserAccessKeypair;
            RequestUri = requestUri;
            SignatureParameters = GetSigBaseStringParams();
            SignatureBaseString = GetSignatureBaseString();
            SigningKey = new SigningKey(ConsumerKeypair, UserAccessKeypair, SignatureBaseString);
            SignedSignature = Cryptography.GetSignature(SigningKey);
        }

        public SigningKey SigningKey { get; set; }
        public Keypair AccessKeypair { get; set; }
        public string SignedSignature { get; private set; }
        public string RequestUri { get; }

        private string GetSigBaseStringParams()
        {
            var SigBaseStringParams = "oauth_consumer_key=" + ConsumerKeypair.PublicKey;
            SigBaseStringParams += "&" + "oauth_nonce=" + Nonce;
            SigBaseStringParams += "&" + "oauth_signature_method=HMAC-SHA1";
            SigBaseStringParams += "&" + "oauth_timestamp=" + Timestamp;
            SigBaseStringParams += "&" + "oauth_token=" + AccessKeypair.PublicKey;
            SigBaseStringParams += "&" + "oauth_version=1.0";
            return SigBaseStringParams;
        }

        private string GetSignatureBaseString()
        {
            var SigBaseString = "GET&";
            SigBaseString += Uri.EscapeDataString(RequestUri) + "&" + Uri.EscapeDataString(SignatureParameters);
            return SigBaseString;
        }
    }
}