using System.ComponentModel.DataAnnotations;

namespace SimpleECommerce.vModels
{
    public class addItemToCartModel
    {
        [Required]
        public int variationId { get; set; } // variaitonId
        [Required]
        [Range(0, 100, ErrorMessage = "Quantity must be between 0 and 100.")]
        public int quantity { get; set; }
    }
}
