namespace TwitterAutherino.Model
{
    internal static class KnownApiUri
    {
        public const string RequestToken = "https://api.twitter.com/oauth/request_token";
        public const string AccessToken = "https://api.twitter.com/oauth/access_token";
        public const string VerifyCredentials = "https://api.twitter.com/1.1/account/verify_credentials.json";
        public const string DefaultCallback = "http://mobile.twitter.com";
    }
}
