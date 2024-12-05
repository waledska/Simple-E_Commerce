using SimpleECommerce.vModels;
using SimpleECommerce.DataAndContext;
using SimpleECommerce.DataAndContext.ModelsForEommerce;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace SimpleECommerce.Services
{
    public class ProdService : IProdService
    {
        private readonly ITransferPhotosToPathWithStoreService _transferPhotosToPath;
        private readonly ApplicationDbContext _dbContext;

        public ProdService(ITransferPhotosToPathWithStoreService transferPhoto, ApplicationDbContext dbContext)
        {
            _transferPhotosToPath = transferPhoto;
            _dbContext = dbContext;
        }

        public async Task<CategoryModel> CreateCategoryAsync(string value)
        {
            if (string.IsNullOrEmpty(value))
                return new CategoryModel { message = "category name can't be empty!" };

            if (await _dbContext.Categories.FirstOrDefaultAsync(c => c.Value == value) != null)
                return new CategoryModel { message = "there is already category with this name!" };

            var newCategory = new Category
            {
                Value = value
            };

            await _dbContext.Categories.AddAsync(newCategory);
            await _dbContext.SaveChangesAsync();

            return new CategoryModel
            {
                Id = newCategory.Id,
                value = newCategory.Value,
                message = ""
            };
        }

        public async Task<IEnumerable<Category>> GetCategoriesAsync()
        {
            return await _dbContext.Categories.ToListAsync();
        }

        public async Task<CategoryModel> UpdateCategoryAsync(CategoryModel model)
        {
            var theCat = await _dbContext.Categories.FirstOrDefaultAsync(x => x.Id == model.Id);
            if (theCat == null)
                return new CategoryModel { message = "invalid category Id!" };

            if (theCat.Value == model.value)
                return new CategoryModel { message = "this is the same name for this category!" };

            // var oldOneWithSameName = await _dbContext.Categories.FirstOrDefaultAsync(c => c.Value == model.value);
            // if (oldOneWithSameName != null)
            //     return new CategoryModel { message = "this new category name is already used for another one!" };

            theCat.Value = model.value;
            _dbContext.Categories.Update(theCat);
            await _dbContext.SaveChangesAsync();

            model.message = "";
            return model;
        }

        public async Task<string> DeleteCategoryAsync(int catId)
        {
            var cat = await _dbContext.Categories.FirstOrDefaultAsync(x => x.Id == catId);
            if (cat == null)
                return "invalid category Id ";

            if (await _dbContext.Products.FirstOrDefaultAsync(x => x.categoryId == catId) != null)
                return "couldn't delete this category before deletting all related products first!";

            _dbContext.Categories.Remove(cat);
            await _dbContext.SaveChangesAsync();

            return "";
        }

        public async Task<ProductResponseModel> CreateProductAsync(ProductRequestModel model)
        {
            if (await _dbContext.Categories.FirstOrDefaultAsync(c => c.Id == model.categoryId) == null)
                return new ProductResponseModel { message = "invalid category Id, there is no categories with this id!" };

            var product = new Product
            {
                Name = model.Name,
                Description = model.Description,
                Price = model.Price,
                categoryId = model.categoryId
            };

            _dbContext.Products.Add(product);
            await _dbContext.SaveChangesAsync();

            return await GetProductByIdAsync(product.Id);
        }

        public async Task<IEnumerable<ProductResponseModel>> ShowProductsAsync()
        {
            var products = await _dbContext.Products
                .Where(p => !p.isDeleted) // Exclude deleted products
                .Include(p => p.Category)
                .Include(p => p.ProductVariations.Where(v => !v.isDeleted)) // Exclude deleted variations
                .ThenInclude(v => v.Color)
                .Include(p => p.ProductVariations.Where(v => !v.isDeleted))
                .ThenInclude(v => v.Size)
                .ToListAsync();

            return products.Select(MapToProductResponse);
        }


        public async Task<ProductResponseModel> UpdateProductAsync(int productId, ProductRequestModel model)
        {
            var product = await _dbContext.Products.FindAsync(productId);
            if (product == null || product.isDeleted)
                return new ProductResponseModel { message = "Invalid product ID or product is deleted." };
            if (await _dbContext.Categories.FirstOrDefaultAsync(c => c.Id == model.categoryId) == null)
                return new ProductResponseModel { message = "invalid category Id, there is no categories with this id!" };

            product.Name = model.Name;
            product.Description = model.Description;
            product.Price = model.Price;
            product.categoryId = model.categoryId;

            await _dbContext.SaveChangesAsync();
            return await GetProductByIdAsync(productId);
        }


        public async Task<bool> DeleteProductAsync(int productId)
        {
            // first i need to modify to move column mainProdVarPhoto from orders to ProdVariations

            // there is two cases: 
            // first product there is orders or in cart for this product(ProductVariation assigned to old order or cart row) => you should soft delete the assigned variation and delete all images only ignore main images for soft deleted variations and product and hard delete for other variaitons those doesn't assigned and delete all thiere photos also
            // there is no orders or not assigned in any cart for this product physical delete for ProdVariaitons and Product and images for this Product


            // check this product is variations has any orders??



            return true;
        }

        // public async Task<bool> DeleteProductAsync(int productId)
        // {
        //     // Find the product
        //     var product = await _dbContext.Products.FindAsync(productId);
        //     if (product == null)
        //         return false;

        //     // Check if any order exists for this product's variations
        //     bool productHasOrders = await _dbContext.OrderRows
        //         .AnyAsync(or => or.ProductVariation.ProductId == productId);

        //     // If the product has orders, set `isDeleted = true` for the product
        //     if (productHasOrders)
        //     {
        //         product.isDeleted = true;
        //     }
        //     else
        //     {
        //         // Get all product variations for this product
        //         var productVariations = await _dbContext.ProductVariations
        //             .Where(v => v.ProductId == productId)
        //             .ToListAsync();

        //         // Get the paths of photos that should not be deleted (referenced in OrderRows)
        //         var photosPathsNotAllowedToDelete = await _dbContext.OrderRows
        //             .Select(or => or.MainProductVariationPhoto)
        //             .ToListAsync();

        //         // Loop through each product variation
        //         foreach (var variation in productVariations)
        //         {
        //             // Check if the variation has any associated orders
        //             bool variationHasOrders = await _dbContext.OrderRows
        //                 .AnyAsync(or => or.ProductVariationId == variation.Id);

        //             // If the variation has orders, set `isDeleted = true`
        //             if (variationHasOrders)
        //             {
        //                 variation.isDeleted = true;
        //             }
        //             else
        //             {
        //                 // Get all photos for this variation that are not referenced in orders
        //                 var photosToDelete = await _dbContext.Photos
        //                     .Where(p => p.ProductVariationId == variation.Id && !photosPathsNotAllowedToDelete.Contains(p.Path))
        //                     .ToListAsync();

        //                 // Delete the photo files and mark the photo entities for deletion
        //                 foreach (var photo in photosToDelete)
        //                 {
        //                     _transferPhotosToPath.DeleteFile(photo.Path);
        //                     _dbContext.Photos.Remove(photo);
        //                 }

        //                 // Mark the variation for deletion (soft delete)
        //                 variation.isDeleted = true;
        //             }
        //         }
        //         _dbContext.Products.Remove(product);
        //     }

        //     // Save changes to the database
        //     await _dbContext.SaveChangesAsync();
        //     return true;
        // }



        // Product Variation (CRUD)
        public async Task<ProductVariationResponseModel> AddVariationForProdAsync(int productId, ProductVariationRequestModel model)
        {
            // Validate ColorId
            if (await _dbContext.Colors.FirstOrDefaultAsync(c => c.Id == model.ColorId) == null)
                return new ProductVariationResponseModel { message = "Invalid ColorId. There is no color with this ID." };

            // Validate SizeId
            if (await _dbContext.Sizes.FirstOrDefaultAsync(c => c.Id == model.SizeId) == null)
                return new ProductVariationResponseModel { message = "Invalid SizeId. There is no size with this ID." };

            if (await _dbContext.Products.FirstOrDefaultAsync(p => p.Id == productId) == null)
                return new ProductVariationResponseModel { message = "Invalid product Id. There is no products with this ID." };

            // Check if a variation already exists with the same ProductId, ColorId, and SizeId
            var existingVariation = await _dbContext.ProductVariations
                .FirstOrDefaultAsync(v => v.ProductId == productId &&
                                          v.ColorId == model.ColorId &&
                                          v.SizeId == model.SizeId);

            if (existingVariation != null)
            {
                // If the variation is not soft-deleted, return an error message
                if (!existingVariation.isDeleted)
                {
                    return new ProductVariationResponseModel
                    {
                        message = "A variation with the same ProductId, ColorId, and SizeId already exists."
                    };
                }

                // Reactivate the soft-deleted variation
                existingVariation.isDeleted = false;
                existingVariation.QuantityInStock = model.QuantityInStock;
                existingVariation.Sku = model.Sku;

                // Handle photo updates for the reactivated variation
                if (model.photosFiles?.Any() == true)
                {
                    var newPhotosPaths = _transferPhotosToPath.GetPhotosPath(model.photosFiles);
                    foreach (var path in newPhotosPaths)
                    {
                        if (path.StartsWith("error, "))
                            return new ProductVariationResponseModel { message = path };

                        // Check if the photo already exists before adding
                        var existingPhoto = await _dbContext.Photos
                            .FirstOrDefaultAsync(p => p.ProductVariationId == existingVariation.Id && p.Path == path);

                        if (existingPhoto == null)
                        {
                            _dbContext.Photos.Add(new Photo { ProductVariationId = existingVariation.Id, Path = path });
                        }
                    }
                }

                await _dbContext.SaveChangesAsync();
                return MapToVariationResponse(existingVariation);
            }

            // If no existing variation is found, create a new one
            var productVariation = new ProductVariation
            {
                ProductId = productId,
                ColorId = model.ColorId,
                SizeId = model.SizeId,
                QuantityInStock = model.QuantityInStock,
                Sku = model.Sku
            };

            _dbContext.ProductVariations.Add(productVariation);
            // i must store this prod variation first get his id from the db then store the pathes for images
            await _dbContext.SaveChangesAsync();

            // Store the images for the new variation
            if (model.photosFiles?.Any() == true)
            {
                var pathes = _transferPhotosToPath.GetPhotosPath(model.photosFiles);
                foreach (var path in pathes)
                {
                    if (path.StartsWith("error, "))
                    {
                        // delete the added new prodVariation again
                        _dbContext.ProductVariations.Remove(productVariation);
                        await _dbContext.SaveChangesAsync();
                        return new ProductVariationResponseModel { message = path };
                    }

                    _dbContext.Photos.Add(new Photo { ProductVariationId = productVariation.Id, Path = path });
                }
            }

            await _dbContext.SaveChangesAsync();

            var result = MapToVariationResponse(productVariation);
            result.ImagesPathes = productVariation.Photos?.Select(p => p.Path).ToList() ?? new List<string>();

            return result;
        }





        public async Task<IEnumerable<ProductVariationResponseModel>> ShowVariationsForProductAsync(int productId)
        {
            var variations = await _dbContext.ProductVariations
                .Where(v => v.ProductId == productId && !v.isDeleted) // Exclude deleted variations
                .Include(v => v.Photos)
                .Include(v => v.Color)
                .Include(v => v.Size)
                .ToListAsync();

            return variations.Select(MapToVariationResponse);
        }

        public async Task<ProductVariationResponseModel> UpdateVariationForProdAsync(int variationId, ProductVariationRequestModel model)
        {
            var variation = await _dbContext.ProductVariations.FindAsync(variationId);
            if (variation == null || variation.isDeleted)
                return new ProductVariationResponseModel { message = "Variation not found or it is deleted." };

            // update photos for prodVariation 
            // 1- try to add the new photos first 
            var theNewPathes = _transferPhotosToPath.GetPhotosPath(model.photosFiles);
            foreach (var path in theNewPathes)
            {
                if (path.StartsWith("error, "))
                {
                    return new ProductVariationResponseModel { message = path };
                }
                _dbContext.Photos.Add(new Photo { ProductVariationId = variationId, Path = path });
            }
            // 2- then delete the old images for this prodVariation if them didn't use in any main order photos
            // avoid nessesary photos 
            var photosPathsNotAllowedToDelete = await _dbContext.OrderRows
                    .Select(or => or.MainProductVariationPhoto)
                    .ToListAsync();

            var photosToDelete = await _dbContext.Photos
                .Where(p => p.ProductVariationId == variationId && !photosPathsNotAllowedToDelete.Contains(p.Path))
                .ToListAsync();

            foreach (var photo in photosToDelete)
            {
                _transferPhotosToPath.DeleteFile(photo.Path);
                _dbContext.Photos.Remove(photo);
            }

            variation.ColorId = model.ColorId;
            variation.SizeId = model.SizeId;
            variation.QuantityInStock = model.QuantityInStock;
            variation.Sku = model.Sku;

            await _dbContext.SaveChangesAsync();

            var result = MapToVariationResponse(variation);
            result.ImagesPathes = variation.Photos?.Select(p => p.Path).ToList() ?? new List<string>();

            return result;
        }


        public async Task<bool> DeleteVariationForProdAsync(int variationId)
        {
            var variation = await _dbContext.ProductVariations.FindAsync(variationId);
            if (variation == null) return false;

            bool variationHasOrders = await _dbContext.OrderRows
                .AnyAsync(or => or.ProductVariationId == variationId);

            if (variationHasOrders)
            {
                // Soft delete the variation
                variation.isDeleted = true;
            }
            else
            {
                // Physically delete the variation
                _dbContext.ProductVariations.Remove(variation);

                var photosPathsNotAllowedToDelete = await _dbContext.OrderRows
                    .Select(or => or.MainProductVariationPhoto)
                    .ToListAsync();

                var photosToDelete = await _dbContext.Photos
                    .Where(p => p.ProductVariationId == variationId && !photosPathsNotAllowedToDelete.Contains(p.Path))
                    .ToListAsync();

                foreach (var photo in photosToDelete)
                {
                    _transferPhotosToPath.DeleteFile(photo.Path);
                    _dbContext.Photos.Remove(photo);
                }
            }

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<ProductVariationResponseModel> ReactivateVariationAsync(int variationId, int? quantityInStock = null, string? sku = null, List<IFormFile>? photosFiles = null)
        {
            var variation = await _dbContext.ProductVariations.FindAsync(variationId);
            if (variation == null || !variation.isDeleted)
                return null;

            // Reactivate the variation
            variation.isDeleted = false;

            // Update stock and SKU if provided
            if (quantityInStock.HasValue)
                variation.QuantityInStock = quantityInStock.Value;

            if (!string.IsNullOrEmpty(sku))
                variation.Sku = sku;

            // Optionally update photos if new ones are provided
            if (photosFiles?.Any() == true)
            {
                var newPhotosPaths = _transferPhotosToPath.GetPhotosPath(photosFiles);
                foreach (var path in newPhotosPaths)
                {
                    if (path.StartsWith("error, "))
                        return new ProductVariationResponseModel { message = path };

                    var existingPhoto = await _dbContext.Photos
                        .FirstOrDefaultAsync(p => p.ProductVariationId == variation.Id && p.Path == path);

                    if (existingPhoto == null)
                    {
                        _dbContext.Photos.Add(new Photo { ProductVariationId = variation.Id, Path = path });
                    }
                }
            }

            await _dbContext.SaveChangesAsync();
            return MapToVariationResponse(variation);
        }


        public async Task<IEnumerable<GetProductsWithColorsResponse>> GetProductsWithColorsAsync()
        {
            var prodWithColors = await _dbContext.Products
            .Where(p => !p.isDeleted)
            .Include(p => p.Category)
            .Include(p => p.ProductVariations)
                .ThenInclude(v => v.Color)
            .Include(p => p.ProductVariations)
                .ThenInclude(v => v.Photos)
            .Select(prod => new
            {
                ProductId = prod.Id,
                Name = prod.Name,
                Description = prod.Description,
                Price = prod.Price,
                Category = prod.Category,
                Colors = prod.ProductVariations
                    .Where(v => !v.isDeleted) // Filter out soft-deleted variations
                    .GroupBy(v => new { v.ColorId, v.Color.Value }) // Group by distinct color
                    .Select(group => new
                    {
                        ColorId = group.Key.ColorId,
                        ColorValue = group.Key.Value,
                        Photos = group.SelectMany(s => s.Photos).ToList() // Fetch all photos for post-processing
                    })
                    .ToList()
            })
            .ToListAsync();

            // Process the photo paths in memory
            var result = prodWithColors.Select(prod => new GetProductsWithColorsResponse
            {
                ProductId = prod.ProductId,
                ProductName = prod.Name,
                Description = prod.Description,
                Price = prod.Price,
                Category = new CategoryProductsResponse
                {
                    Id = prod.Category.Id,
                    value = prod.Category.Value
                },
                Colors = prod.Colors.Select(color => new color
                {
                    ColorId = color.ColorId,
                    ColorValue = color.ColorValue,
                    ProductPhotoForThisColor = color.Photos.FirstOrDefault()?.Path ?? "There are no images for this color!"
                })
                .ToList()
            });


            return result;
        }

        // Colors [C - R]
        public async Task<AddColorResponseModel> addColor(string value)
        {
            if (await _dbContext.Colors.FirstOrDefaultAsync(c => c.Value == value) != null)
                return new AddColorResponseModel { message = "this color value is already stored!" };

            // storing the new color 
            var newColor = new Color
            {
                Value = value
            };
            await _dbContext.Colors.AddAsync(newColor);
            await _dbContext.SaveChangesAsync();
            return new AddColorResponseModel { message = "", Id = newColor.Id, Value = newColor.Value };
        }

        public async Task<List<Color>> getColors()
        {
            return await _dbContext.Colors.ToListAsync();
        }
        public async Task<List<Size>> getSizes()
        {
            return await _dbContext.Sizes.ToListAsync();
        }


        // Helper Methods
        private async Task<ProductResponseModel> GetProductByIdAsync(int productId)
        {
            var product = await _dbContext.Products
                .Where(p => !p.isDeleted && p.Id == productId) // Exclude soft-deleted products
                .Include(p => p.Category)
                .Include(p => p.ProductVariations.Where(v => !v.isDeleted)) // Exclude soft-deleted variations
                .ThenInclude(v => v.Color)
                .Include(p => p.ProductVariations.Where(v => !v.isDeleted))
                .ThenInclude(v => v.Size)
                .FirstOrDefaultAsync();

            if (product == null) return null;

            return MapToProductResponse(product);
        }


        private ProductResponseModel MapToProductResponse(Product product) =>
            new ProductResponseModel
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                category = new CategoryProductsResponse
                {
                    Id = product.Category.Id,
                    value = product.Category.Value
                },
                Variations = product.ProductVariations
                .Where(v => !v.isDeleted) // Exclude soft-deleted variations
                .Select(MapToVariationResponse)
                .ToList()
            };



        private ProductVariationResponseModel MapToVariationResponse(ProductVariation variation) =>
        new ProductVariationResponseModel
        {
            Id = variation.Id,
            Color = variation.Color?.Value ?? "N/A",
            Size = variation.Size?.Value ?? "N/A",
            QuantityInStock = variation.QuantityInStock,
            Sku = variation.Sku,
            ImagesPathes = variation.Photos?.Select(p => p.Path).ToList() ?? new List<string>(),
            message = ""
        };
    }
}
