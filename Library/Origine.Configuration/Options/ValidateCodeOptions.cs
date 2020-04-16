namespace Origine
{
    public class ValidateCodeOptions
    {
        public string Url { get; set; }
        public string Message { get; set; }
        public string Subcode { get; set; }
        public int Length { get; set; }
        public int SecondsOfExpiredTime { get; set; }
        public int SecondsOfClearTime { get; set; }
    }
}
