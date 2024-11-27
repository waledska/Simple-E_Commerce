using System.ComponentModel.DataAnnotations;

namespace SimpleECommerce.vModels
{
    public class sendOTPForLoginModel
    {
        [Required]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Invalid email format.")]
        public string email { get; set; }
        [Required]
        public string userName { get; set; }
    }
}
