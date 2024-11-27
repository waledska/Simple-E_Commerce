namespace SimpleECommerce.vModels
{
    public class resetPassTokenResult
    {
        public string message { get; set; }
        public string? OTP { get; set; }
        public resetPassTokenResult()
        {
            message = string.Empty;
        }
    }
}
