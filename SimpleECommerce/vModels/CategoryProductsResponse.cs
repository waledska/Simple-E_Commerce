using System.ComponentModel.DataAnnotations;

namespace SimpleECommerce.vModels
{
    public class CategoryProductsResponse
    {
        public CategoryProductsResponse()
        {
            value = string.Empty;
        }

        public int Id { get; set; }
        public string value { get; set; }
    }
}
