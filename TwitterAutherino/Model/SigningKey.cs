namespace TwitterAutherino.Model
{
    public class SigningKey
    {
        public SigningKey(Keypair consumerKeypair, Keypair clientKeypair, string signatureParameters)
        {
            ConsumerKeypair = consumerKeypair;
            ClientKeypair = clientKeypair;
            SignatureParameters = signatureParameters;
        }

        public SigningKey(Keypair consumerKeypair, string signatureParameters)
            : this(consumerKeypair, null, signatureParameters)
        {
        }

        public Keypair ConsumerKeypair { get; set; }
        public Keypair ClientKeypair { get; set; }
        public string SignatureParameters { get; set; }

        public string GetSigningKey()
        {
            if (ClientKeypair != null &&
                !string.IsNullOrWhiteSpace(ClientKeypair.SecretKey))
            {
                return ConsumerKeypair.SecretKey + "&" + ClientKeypair.SecretKey;
            }
            return ConsumerKeypair.SecretKey + "&";
        }
    }
}