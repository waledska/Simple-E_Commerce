namespace SimpleECommerce.vModels
{
    public class registerResult
    {
        public string name { get; set; }
        public string email { get; set; }
        public string token { get; set; }
        public DateTime? ExpiresOn { get; set; }
        public string Message { get; set; }
    }
}
