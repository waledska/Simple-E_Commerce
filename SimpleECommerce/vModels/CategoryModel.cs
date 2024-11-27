using System.ComponentModel.DataAnnotations;

namespace SimpleECommerce.vModels
{
    public class CategoryModel
    {
        public CategoryModel()
        {
            value = string.Empty;
            message = string.Empty;
        }
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string value { get; set; }
        public string message { get; set; }

    }
}
