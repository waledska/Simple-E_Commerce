using System.ComponentModel.DataAnnotations;

namespace SimpleECommerce.vModels
{
    public class ProductVariationRequestModel
    {
        [Required]
        public int ColorId { get; set; }

        [Required]
        public int SizeId { get; set; }

        [Required]
        [Range(0, 9999)]
        public int QuantityInStock { get; set; }

        [MaxLength(100)]
        public string? Sku { get; set; }
        [DataType(DataType.Upload)]
        public List<IFormFile> photosFiles { get; set; }
    }
}
