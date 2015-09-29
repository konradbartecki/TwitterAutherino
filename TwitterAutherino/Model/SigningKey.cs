namespace TwitterAutherino.Model
{
    public class SigningKey
    {
        public Keypair ConsumerKeypair { get; set; }
        public Keypair ClientKeypair { get; set; }
        public string SignatureParameters { get; set; }

        public SigningKey(Keypair consumerKeypair, Keypair clientKeypair, string signatureParameters)
        {
            this.ConsumerKeypair = consumerKeypair;
            this.ClientKeypair = clientKeypair;
            this.SignatureParameters = signatureParameters;
        }

        public SigningKey(Keypair consumerKeypair, string signatureParameters) : this(consumerKeypair, null, signatureParameters)
        {
        }

        public string GetSigningKey()
        {
            if (ClientKeypair != null && 
                !string.IsNullOrWhiteSpace(ClientKeypair.SecretKey))
            {
                return this.ConsumerKeypair.SecretKey + "&" + ClientKeypair.SecretKey;
            }
            else
            {
                return this.ConsumerKeypair.SecretKey + "&";
            }
        }
    }
}
