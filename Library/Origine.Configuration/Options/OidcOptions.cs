namespace Origine
{
    public class OidcOptions
    {
        public string UrlPrefix { get; set; }
        public string CheckLoginUrl { get; set; }
        public string EditUserUrl { get; set; }
        public string ServerKey { get; set; }

        public string WholeCheckLoginUrl => UrlPrefix + CheckLoginUrl;
        public string WholeEditUserUrl => UrlPrefix + EditUserUrl;
    }
}
