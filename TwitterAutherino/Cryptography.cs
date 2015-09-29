using System;
using System.Text;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;
using TwitterAutherino.Model;

namespace TwitterAutherino
{
    public static class Cryptography
    {
        public static string GetNonce()
        {
            var builder = new StringBuilder();
            var random = new Random();
            char ch;
            for (var i = 0; i < 32; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26*random.NextDouble() + 65)));
                builder.Append(ch);
            }

            return builder.ToString().ToLower();
        }

        public static string GetTimeStamp()
        {
            var unixTimestamp = (int) (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            return unixTimestamp.ToString();
        }

        internal static string GetSignature(SigningKey signingKey)
        {
            IBuffer KeyMaterial;
            KeyMaterial = CryptographicBuffer.ConvertStringToBinary(signingKey.GetSigningKey(),
                BinaryStringEncoding.Utf8);
            var HmacSha1Provider = MacAlgorithmProvider.OpenAlgorithm("HMAC_SHA1");
            var MacKey = HmacSha1Provider.CreateKey(KeyMaterial);
            var DataToBeSigned = CryptographicBuffer.ConvertStringToBinary(signingKey.SignatureParameters,
                BinaryStringEncoding.Utf8);
            var SignatureBuffer = CryptographicEngine.Sign(MacKey, DataToBeSigned);
            var Signature = CryptographicBuffer.EncodeToBase64String(SignatureBuffer);

            return Signature;
        }
    }
}