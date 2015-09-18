using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;

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


        public static string GetTimeStamp()
        {
            Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            return unixTimestamp.ToString();
        }

        public static string GetSignature(string sigBaseString, string consumerSecretKey)
        {
            return GetSignature(sigBaseString, consumerSecretKey, false);
        }

        public static string GetSignature(string sigBaseString, string consumerSecretKey, bool isLongSigningKey)
        {
            Windows.Storage.Streams.IBuffer KeyMaterial;
            if (isLongSigningKey)
                KeyMaterial = CryptographicBuffer.ConvertStringToBinary(consumerSecretKey, BinaryStringEncoding.Utf8);
            else
                KeyMaterial = CryptographicBuffer.ConvertStringToBinary(consumerSecretKey + "&", BinaryStringEncoding.Utf8);
            MacAlgorithmProvider HmacSha1Provider = MacAlgorithmProvider.OpenAlgorithm("HMAC_SHA1");
            CryptographicKey MacKey = HmacSha1Provider.CreateKey(KeyMaterial);
            Windows.Storage.Streams.IBuffer DataToBeSigned = CryptographicBuffer.ConvertStringToBinary(sigBaseString, BinaryStringEncoding.Utf8);
            Windows.Storage.Streams.IBuffer SignatureBuffer = CryptographicEngine.Sign(MacKey, DataToBeSigned);
            string Signature = CryptographicBuffer.EncodeToBase64String(SignatureBuffer);

            return Signature;
        }
    }
}
