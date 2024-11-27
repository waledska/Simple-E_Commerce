
using SimpleECommerce.DataAndContext.ModelsForEommerce;

namespace SimpleECommerce.vModels
{
    public class GetProductsWithColorsResponse
    {
        public GetProductsWithColorsResponse()
        {
            ProductName = "";
            Description = "";
            Colors = new List<color>();
            Category = new CategoryProductsResponse();
        }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal Price { get; set; }
        public CategoryProductsResponse Category { get; set; }
        public List<color> Colors { get; set; }
    }

    public class color
    {
        public color()
        {
            ColorValue = "";
            ProductPhotoForThisColor = "";
        }
        public int ColorId { get; set; }
        public string ColorValue { get; set; } = null!;
        public string ProductPhotoForThisColor { get; set; } = null!;
    }
}


