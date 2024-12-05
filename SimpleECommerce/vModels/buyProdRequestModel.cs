using System.ComponentModel.DataAnnotations;
using SimpleECommerce.DataAndContext.ModelsForEommerce;

namespace SimpleECommerce.vModels
{
    public class buyProdRequestModel
    {
        [Required]
        public ProductVariation productVariation { get; set; }
        [Required]
        [Range(0, 100, ErrorMessage = "Quantity must be between 0 and 100.")]
        public int quantity { get; set; }
        [Required]
        public int addressId { get; set; }
    }
}
