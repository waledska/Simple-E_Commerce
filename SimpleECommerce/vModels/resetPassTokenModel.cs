using System.ComponentModel.DataAnnotations;

namespace SimpleECommerce.vModels
{
    public class resetPassTokenModel
    {
        [Required]
        public string gmailOrPhone { get; set; }
    }
}
