using System.ComponentModel.DataAnnotations;

namespace SimpleECommerce.vModels
{
    public class resetPassModel
    {
        [Required]
        public string gmail { get; set; }
        [Required]
        public string OTP { get; set; }
        [Required]
        public string newPassword { get; set; }
        [Required]
        public string confirmNewPassword { get; set; }
    }
}
