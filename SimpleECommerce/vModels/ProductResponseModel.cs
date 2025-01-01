using System.ComponentModel.DataAnnotations;
using SimpleECommerce.DataAndContext.ModelsForEommerce;

namespace SimpleECommerce.vModels
{
    public class ProductResponseModel
    {
        public ProductResponseModel()
        {
            message = "";
            Variations = new List<ProductVariationResponseModel>();
            category = new CategoryProductsResponse();
        }
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public bool isDeleted { get; set; }
        public decimal Price { get; set; }
        public List<ProductVariationResponseModel> Variations { get; set; } = new();
        public string message { get; set; }
        public CategoryProductsResponse category { get; set; }
    }
}
