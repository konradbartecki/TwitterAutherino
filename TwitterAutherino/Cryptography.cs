using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using TwitterAutherino.Model;

namespace TwitterAutherino
{
    public static class Cryptography
    {
        public static string GetNonce()
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;
            for (int i = 0; i < 32; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }

            return builder.ToString().ToLower();
        }

        //public static string GetSigBaseStringParams()
        //{
            
        //}

        internal static string GetBaseStringSignature(TwitterRequest request)
        {
            //Consumer key, timestamp, nonce mandatory
            //Token in 2 of 3 scenarios
            //Callback url in 1 scenario
            string SigBaseStringParams = "";
            if (request.RequestType == RequestType.GetRequestToken)
            {
                SigBaseStringParams = "oauth_callback=" + Uri.EscapeDataString(request.CallbackAddress);
                SigBaseStringParams += "&" + "oauth_consumer_key=" + request.ConsumerKeypair.PublicKey;
            }
            else
            {
                SigBaseStringParams += "oauth_consumer_key=" + request.ConsumerKeypair.PublicKey;
            }                           
            SigBaseStringParams += "&" + "oauth_nonce=" + request.Nonce;
            SigBaseStringParams += "&" + "oauth_signature_method=HMAC-SHA1";
            SigBaseStringParams += "&" + "oauth_timestamp=" + request.Timestamp;

            switch (request.RequestType)
            {
                case RequestType.GeneralGet:
                        SigBaseStringParams += "&" + "oauth_token=" + request.AccessKeypair.PublicKey;
                    break;
                case RequestType.GetAccessToken:
                        SigBaseStringParams += "&" + "oauth_token=" + request.ConsumerKeypair.SecretKey;
                    break;
            }
            SigBaseStringParams += "&" + "oauth_version=1.0";
            string SigBaseString;
            switch (request.RequestType)
            {
                case RequestType.GetAccessToken:
                    SigBaseString = "POST&";
                    break;
                default:
                    SigBaseString = "GET&";
                    break;
            }
            SigBaseString += Uri.EscapeDataString(request.RequestAddress) + "&" + Uri.EscapeDataString(SigBaseStringParams);
            return SigBaseString;
        }


        public static string GetTimeStamp()
        {
            Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            return unixTimestamp.ToString();
        }

        internal static string GetSignature(TwitterRequest request)
        {
            Windows.Storage.Streams.IBuffer KeyMaterial;

            switch (request.RequestType)
            {
                case RequestType.GeneralGet:
                    string signingkey = request.ConsumerKeypair.SecretKey + "&" + request.AccessKeypair.SecretKey;
                    KeyMaterial = CryptographicBuffer.ConvertStringToBinary(signingkey, BinaryStringEncoding.Utf8);
                    break;
                default:
                    KeyMaterial = CryptographicBuffer.ConvertStringToBinary(request.ConsumerKeypair.SecretKey + "&", BinaryStringEncoding.Utf8);
                    break;

            }
            MacAlgorithmProvider HmacSha1Provider = MacAlgorithmProvider.OpenAlgorithm("HMAC_SHA1");
            CryptographicKey MacKey = HmacSha1Provider.CreateKey(KeyMaterial);
            Windows.Storage.Streams.IBuffer DataToBeSigned = CryptographicBuffer.ConvertStringToBinary(request.UnsignedParametrsString, BinaryStringEncoding.Utf8);
            Windows.Storage.Streams.IBuffer SignatureBuffer = CryptographicEngine.Sign(MacKey, DataToBeSigned);
            string Signature = CryptographicBuffer.EncodeToBase64String(SignatureBuffer);

            return Signature;
        }
    }
}
