using System.ComponentModel.DataAnnotations;

namespace SimpleECommerce.vModels
{
    public class loginModel
    {
        [Required]
        public string UserPassword { get; set; }
        [Required]
        public string userEmail { get; set; }
    }
}
