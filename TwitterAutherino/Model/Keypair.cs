namespace TwitterAutherino.Model
{
    public class Keypair
    {
        public Keypair(string KeyOrToken, string secretToken)
        {
            PublicKey = KeyOrToken;
            SecretKey = secretToken;
        }

        public string PublicKey { get; set; }
        public string SecretKey { get; set; }
    }
}