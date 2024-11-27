using System.ComponentModel.DataAnnotations;

namespace SimpleECommerce.vModels
{
    public class ProductRequestModel
    {
        [Required]
        [MaxLength(255)]
        public string Name { get; set; } = null!;

        [Required]
        [MaxLength(1000)]
        public string Description { get; set; } = null!;

        [Required]
        [Range(0.01, 99999.99)]
        public decimal Price { get; set; }

        [Required]
        public int categoryId { get; set; }


        //public List<ProductVariationRequestModel> Variations { get; set; } = new();
    }
}
