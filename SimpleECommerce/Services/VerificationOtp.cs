using System.ComponentModel.DataAnnotations;

namespace SimpleECommerce.vModels
{
    public class VerificationOtp
    {
        public string userEmail { get; set; }
        public string OTP { get; set; }
    }
}
