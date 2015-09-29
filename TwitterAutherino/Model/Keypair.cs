namespace TwitterAutherino.Model
{
    public class Keypair
    {
        public string PublicKey { get; set; }
        public string SecretKey { get; set; }

        public Keypair(string KeyOrToken, string secretToken)
        {
            this.PublicKey = KeyOrToken;
            this.SecretKey = secretToken;
        }
    }
}
