using SimpleECommerce.DataAndContext.ModelsForEommerce;
using SimpleECommerce.vModels;

namespace SimpleECommerce.Services
{
    public interface IProdService
    {
        // Category [C R U D]
        Task<CategoryModel> CreateCategoryAsync(string value);
        Task<IEnumerable<Category>> GetCategoriesAsync();
        Task<CategoryModel> UpdateCategoryAsync(CategoryModel model);
        Task<string> DeleteCategoryAsync(int catId);

        // Product (CRUD)
        Task<ProductResponseModel> CreateProductAsync(ProductRequestModel model);
        Task<IEnumerable<ProductResponseModel>> ShowProductsAsync();
        Task<ProductResponseModel> UpdateProductAsync(int productId, ProductRequestModel model);
        Task<bool> DeleteProductAsync(int productId);
        Task<string> ReactivateProductAsync(int productId);

        // Product Variation (CRUD)
        Task<ProductVariationResponseModel> AddVariationForProdAsync(int productId, ProductVariationRequestModel model);
        Task<IEnumerable<ProductVariationResponseModel>> ShowVariationsForProductAsync(int productId);
        Task<ProductVariationResponseModel> UpdateVariationForProdAsync(int variationId, ProductVariationRequestModel model);
        Task<bool> DeleteVariationForProdAsync(int variationId);
        Task<ProductVariationResponseModel> ReactivateVariationAsync(int variationId, int? quantityInStock = null, string? sku = null, List<IFormFile>? photosFiles = null);

        // Additional Functionality
        Task<IEnumerable<GetProductsWithColorsResponse>> GetProductsWithColorsAsync();

        // Color (CR)
        Task<AddColorResponseModel> addColor(string value);
        Task<List<Color>> getColors();
        Task<List<Size>> getSizes();
    }
}
