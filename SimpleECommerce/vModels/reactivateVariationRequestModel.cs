using System.ComponentModel.DataAnnotations;

namespace SimpleECommerce.vModels
{
    public class reactivateVariationRequestModel
    {
        public reactivateVariationRequestModel()
        {
            photosFiles = new List<IFormFile>();
            sku = string.Empty;
        }
        [Required]
        public int variationId { get; set; }
        [Required]
        public int quantityInStock { get; set; }
        public String sku { get; set; }
        [Required]
        public List<IFormFile> photosFiles { get; set; }
    }
}
