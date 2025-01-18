using SimpleECommerce.vModels;
using SimpleECommerce.DataAndContext;
using SimpleECommerce.DataAndContext.ModelsForEommerce;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;


namespace SimpleECommerce.Services
{
    public class ProdService : IProdService
    {
        private readonly ITransferPhotosToPathWithStoreService _transferPhotosToPath;
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<ProdService> _logger;

        public ProdService(ITransferPhotosToPathWithStoreService transferPhoto, ApplicationDbContext dbContext, ILogger<ProdService> logger)
        {
            _transferPhotosToPath = transferPhoto;
            _dbContext = dbContext;
            _logger = logger;
        }

        // Category [C R U D]
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

        // Product (CRUD)
        public async Task<ProductResponseModel> CreateProductAsync(ProductRequestModel model)
        {
            if (await _dbContext.Categories.FirstOrDefaultAsync(c => c.Id == model.categoryId) == null)
                return new ProductResponseModel { message = "invalid category Id, there is no categories with this id!" };

            var oldProd = await _dbContext.Products.FirstOrDefaultAsync(p => p.Name == model.Name && p.categoryId == model.categoryId);
            if (oldProd != null)
            {
                if (oldProd.isDeleted)
                    return new ProductResponseModel { message = "there is already Product stored with this name and categoty reactivate if only" };
                return new ProductResponseModel { message = "there is already Product stored with this name in this categoty" };
            }
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
                //.Where(p => !p.isDeleted) // Exclude deleted products
                .Include(p => p.Category)
                .Include(p => p.ProductVariations/*.Where(v => !v.isDeleted)*/) // Exclude deleted variations
                .ThenInclude(v => v.Color)
                .Include(p => p.ProductVariations/*.Where(v => !v.isDeleted)*/)
                .ThenInclude(v => v.Size)
                .Include(p => p.ProductVariations/*.Where(v => !v.isDeleted)*/)
                .ThenInclude(v => v.Photos)
                .AsNoTracking().ToListAsync();

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

        /// <summary>
        /// Deletes a product either softly or hard based on its associations with orders or carts.
        /// </summary>
        /// <param name="productId">The ID of the product to delete.</param>
        /// <returns>True if the deletion was successful; otherwise, false.</returns>
        public async Task<bool> DeleteProductAsync(int productId)
        {
            var photosToDelete = new List<Photo>();
            // Begin a database transaction to ensure atomicity
            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                // Fetch the product with its variations and photos
                var currentProduct = await _dbContext.Products
                    .Include(p => p.ProductVariations)
                        .ThenInclude(pv => pv.Photos)
                    .FirstOrDefaultAsync(p => p.Id == productId);

                // If the product doesn't exist, return false
                if (currentProduct == null)
                    return false;

                // Get all variation IDs for the product
                var productVarIds = currentProduct.ProductVariations.Select(pv => pv.Id).ToList();

                // Find all variation IDs assigned to any order or cart
                var assignedVariationsIds = await _dbContext.OrderRows
                    .Where(or => productVarIds.Contains(or.ProductVariationId))
                    .Select(or => or.ProductVariationId)
                    .Distinct()
                    .Union(
                        _dbContext.CartRows
                            .Where(cr => productVarIds.Contains(cr.ProductVariationId))
                            .Select(cr => cr.ProductVariationId)
                    )
                    .ToListAsync();

                bool anyAssigned = assignedVariationsIds.Any();

                if (anyAssigned)
                {
                    // Soft delete assigned variations and hard delete unassigned variations
                    foreach (var variation in currentProduct.ProductVariations)
                    {
                        if (assignedVariationsIds.Contains(variation.Id))
                        {
                            // Soft delete the variation
                            variation.isDeleted = true;

                            // Identify photos to delete (excluding the main photo)
                            photosToDelete.AddRange(variation.Photos
                                .Where(ph => ph.Path != variation.MainProductVariationPhoto)
                                .ToList());
                        }
                        else
                        {
                            // Hard delete the variation and all its photos
                            photosToDelete.AddRange(variation.Photos.ToList());

                            _dbContext.ProductVariations.Remove(variation);
                        }
                    }
                    // Remove photos from the database
                    _dbContext.Photos.RemoveRange(photosToDelete);

                    // Soft delete the product
                    currentProduct.isDeleted = true;
                }
                else
                {
                    // No variations are assigned; perform a hard delete => hard delete for product and it's variations

                    // Collect all photos associated with the product's variations
                    photosToDelete = currentProduct.ProductVariations
                        .SelectMany(pv => pv.Photos)
                        .ToList();

                    // Remove photos from the database
                    _dbContext.Photos.RemoveRange(photosToDelete);
                    // Remove all variations
                    _dbContext.ProductVariations.RemoveRange(currentProduct.ProductVariations);

                    // Remove the product
                    _dbContext.Products.Remove(currentProduct);
                }

                // Save all changes to the database
                await _dbContext.SaveChangesAsync();

                // Commit the transaction
                await transaction.CommitAsync();

            }
            catch (Exception ex)
            {
                // Rollback the transaction in case of any failure
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Failed to delete product with ID {ProductId}", productId);

                // Rethrow the exception to be handled by the caller or a global exception handler
                throw;
            }
            // Proceed to delete the photo files after successful transaction commit
            try
            {
                // Delete photo files
                foreach (var photo in photosToDelete)
                {
                    await _transferPhotosToPath.DeleteFileAsync(photo.Path);
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions from file deletions
                _logger.LogError(ex, "One or more photo files could not be deleted for product ID {ProductId}", productId);
                return false;
            }
            // If all operations succeeded, return true
            return true;
        }

        public async Task<string> ReactivateProductAsync(int productId)
        {
            var product = await _dbContext.Products
                                            .Where(p => p.Id == productId)
                                            .Include(pr => pr.ProductVariations)
                                            .FirstOrDefaultAsync();
            if (product == null)
                return "invalid product id, there is no products with this id!";

            product.isDeleted = false;

            // reactivate all product's cariaitons also 
            foreach (var variation in product.ProductVariations)
            {
                variation.isDeleted = false;
            }
            await _dbContext.SaveChangesAsync();
            return "";
        }



        // Product Variation (CRUD)
        public async Task<ProductVariationResponseModel> AddVariationForProdAsync(int productId, ProductVariationRequestModel model)
        {
            // ----- using transaction -----
            var newPhotosPaths = new List<String>();
            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                // quantity in stock can't be less than 0!
                if (model.QuantityInStock < 0)
                    return new ProductVariationResponseModel { message = "quantity for productVariaiton in stock can't be less than 0!" };
                // Validate ColorId
                if (await _dbContext.Colors.FirstOrDefaultAsync(c => c.Id == model.ColorId) == null)
                    return new ProductVariationResponseModel { message = "Invalid ColorId. There is no color with this ID." };

                // Validate SizeId
                if (await _dbContext.Sizes.FirstOrDefaultAsync(c => c.Id == model.SizeId) == null)
                    return new ProductVariationResponseModel { message = "Invalid SizeId. There is no size with this ID." };

                var product = await _dbContext.Products.FirstOrDefaultAsync(p => p.Id == productId);
                if (product == null)
                    return new ProductVariationResponseModel { message = "Invalid product Id. There is no products with this ID." };
                if (product.isDeleted == true)
                    return new ProductVariationResponseModel { message = "please reactivate this product first!" };


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
                    return new ProductVariationResponseModel
                    {
                        message = "this variation data is already stored you need to reactivate it only!"
                    };

                    // // Reactivate the soft-deleted variation ////////------------- i need to remove recativation variaiton from here only in her hethod 

                    // existingVariation.isDeleted = false;
                    // existingVariation.QuantityInStock = model.QuantityInStock;
                    // existingVariation.Sku = model.Sku;


                    // // Handle photo updates for the reactivated variation
                    // if (model.photosFiles?.Any() == true) // ------------------------------------------------------------->
                    // {
                    //     var photosToAdd = new List<Photo>();
                    //     newPhotosPaths = await _transferPhotosToPath.GetPhotosPathAsync(model.photosFiles);
                    //     foreach (var path in newPhotosPaths)
                    //     {
                    //         if (path.StartsWith("error, "))
                    //         {
                    //             if (photosToAdd.Any())
                    //             {
                    //                 foreach (var photoPath in newPhotosPaths)
                    //                 {
                    //                     if (!photoPath.StartsWith("error, "))
                    //                         await _transferPhotosToPath.DeleteFileAsync(photoPath);
                    //                 }
                    //             }
                    //             return new ProductVariationResponseModel { message = path };
                    //         }

                    //         // Check if the photo already exists before adding
                    //         var existingPhoto = await _dbContext.Photos
                    //             .FirstOrDefaultAsync(p => p.ProductVariationId == existingVariation.Id && p.Path == path);

                    //         if (existingPhoto == null)
                    //         {
                    //             photosToAdd.Add(new Photo { ProductVariationId = existingVariation.Id, Path = path });
                    //         }
                    //     }
                    //     await _dbContext.Photos.AddRangeAsync(photosToAdd);
                    //     existingVariation.MainProductVariationPhoto = photosToAdd[0].Path;
                    // }

                    // await _dbContext.SaveChangesAsync();
                    // return MapToVariationResponse(existingVariation);
                }

                // If no existing variation is found, create a new one
                var productVariation = new ProductVariation
                {
                    ProductId = productId,
                    ColorId = model.ColorId,
                    SizeId = model.SizeId,
                    QuantityInStock = model.QuantityInStock,
                    Sku = model.Sku,
                    MainProductVariationPhoto = "No main image for this variaiton!"
                };

                _dbContext.ProductVariations.Add(productVariation);
                // i must store this prod variation first get his id from the db then store the pathes for images
                await _dbContext.SaveChangesAsync();


                // Store the images for the new variation
                if (model.photosFiles?.Any() == true)
                {
                    newPhotosPaths = await _transferPhotosToPath.GetPhotosPathAsync(model.photosFiles);
                    var newPhotos = new List<Photo>();
                    foreach (var path in newPhotosPaths)
                    {
                        if (path.StartsWith("error, "))
                        {   // delete the new added images again
                            foreach (var photoPath in newPhotosPaths)
                            {
                                if (!path.StartsWith("error, "))
                                    await _transferPhotosToPath.DeleteFileAsync(photoPath);
                            }
                            // delete the added new prodVariation again
                            _dbContext.ProductVariations.Remove(productVariation);
                            await _dbContext.SaveChangesAsync();
                            return new ProductVariationResponseModel { message = path };
                        }

                        newPhotos.Add(new Photo { ProductVariationId = productVariation.Id, Path = path });
                    }
                    productVariation.MainProductVariationPhoto = newPhotos[0].Path;
                    _dbContext.ProductVariations.Update(productVariation);
                    await _dbContext.Photos.AddRangeAsync(newPhotos);
                }

                await _dbContext.SaveChangesAsync();

                var result = MapToVariationResponse(productVariation);
                result.ImagesPathes = productVariation.Photos?.Select(p => p.Path).ToList() ?? new List<string>();

                await transaction.CommitAsync();
                return result;
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();

                foreach (var path in newPhotosPaths)
                {
                    if (path.StartsWith("error, "))
                        await _transferPhotosToPath.DeleteFileAsync(path);
                }
                throw;
            }
        }
        public async Task<IEnumerable<ProductVariationResponseModel>> ShowVariationsForProductAsync(int productId)
        {
            var variations = await _dbContext.ProductVariations
                .Where(v => v.ProductId == productId /*&& !v.isDeleted*/) // Exclude deleted variations
                .Include(v => v.Photos)
                .Include(v => v.Color)
                .Include(v => v.Size)
                .ToListAsync();

            return variations.Select(MapToVariationResponse);
        }

        public async Task<ProductVariationResponseModel> UpdateVariationForProdAsync(int variationId, ProductVariationRequestModel model)
        {
            // here you don't need using transaction because only one using for saveChanges

            var variation = await _dbContext.ProductVariations
                .Include(pv => pv.Photos)
                .FirstOrDefaultAsync(pv => pv.Id == variationId);

            if (variation == null || variation.isDeleted)
                return new ProductVariationResponseModel { message = "Variation not found or it is deleted please reactivate it first!" };
            if (model.QuantityInStock < 0)
                return new ProductVariationResponseModel { message = "quantity for productVariaiton in stock can't be less than 0!" };

            // update photos for prodVariation 
            // 1- try to add the new photos first 
            var theNewPathes = await _transferPhotosToPath.GetPhotosPathAsync(model.photosFiles);
            var theNewPhotos = new List<Photo>();
            foreach (var path in theNewPathes)
            {
                if (path.StartsWith("error, "))
                {
                    // delete any stored photos
                    foreach (var photoPath in theNewPathes)
                    {
                        if (!photoPath.StartsWith("error, "))
                            await _transferPhotosToPath.DeleteFileAsync(photoPath);
                    }
                    return new ProductVariationResponseModel { message = path };
                }
                theNewPhotos.Add(new Photo { ProductVariationId = variationId, Path = path });
            }

            // 2- then delete the old images for this prodVariation
            foreach (var photo in variation.Photos)
            {
                await _transferPhotosToPath.DeleteFileAsync(photo.Path);
                _dbContext.Photos.Remove(photo);
            }

            // add the new photos
            _dbContext.Photos.AddRange(theNewPhotos);

            variation.ColorId = model.ColorId;
            variation.SizeId = model.SizeId;
            variation.QuantityInStock = model.QuantityInStock;
            variation.Sku = model.Sku;
            variation.MainProductVariationPhoto = theNewPhotos[0].Path;

            // update cart rows if the new quantity in stock is less than the quantity in cart
            var cartRows = await _dbContext.CartRows
            .Where(cr => cr.ProductVariationId == variationId)
            .ToListAsync();

            if (cartRows.Any())
            {

                foreach (var cartRow in cartRows)
                {
                    // decreasing for the cartRow quantity
                    if (cartRow.Quantity > variation.QuantityInStock)
                        cartRow.Quantity = variation.QuantityInStock;
                    // increasing for the cartRow quantity
                    if (cartRow.Quantity <= 0 && variation.QuantityInStock > 0)
                        cartRow.Quantity = 1;
                }
            }

            await _dbContext.SaveChangesAsync();

            var result = MapToVariationResponse(variation);
            result.ImagesPathes = variation.Photos?.Select(p => p.Path).ToList() ?? new List<string>();

            return result;
        }


        public async Task<bool> DeleteVariationForProdAsync(int variationId)
        {
            var variation = await _dbContext.ProductVariations.Include(pv => pv.Photos).FirstOrDefaultAsync(pv => pv.Id == variationId);
            if (variation == null) return false;

            bool IsVarAssignedInOrderOrCart = await _dbContext.OrderRows
                .AnyAsync(or => or.ProductVariationId == variationId) ||
                await _dbContext.CartRows.AnyAsync(c => c.ProductVariationId == variationId);

            if (IsVarAssignedInOrderOrCart)
            {
                // Soft delete the variation
                variation.isDeleted = true;
                // deleting photos which possible to delete
                foreach (var Photo in variation.Photos)
                {
                    if (Photo.Path != variation.MainProductVariationPhoto)
                    {
                        _dbContext.Photos.Remove(Photo);
                        await _transferPhotosToPath.DeleteFileAsync(Photo.Path);
                    }
                }
            }
            else
            {
                // Hard delete the variation
                _dbContext.ProductVariations.Remove(variation);

                // var photosPathsNotAllowedToDelete = await _dbContext.OrderRows
                //     .Select(or => or.MainProductVariationPhoto)
                //     .ToListAsync();

                foreach (var photo in variation.Photos)
                {
                    await _transferPhotosToPath.DeleteFileAsync(photo.Path);
                }
                // delete photos pathes from db
                _dbContext.Photos.RemoveRange(variation.Photos);
            }

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<ProductVariationResponseModel> ReactivateVariationAsync(reactivateVariationRequestModel model)
        {
            var variation = await _dbContext.ProductVariations
                            .Include(v => v.Color)
                            .Include(v => v.Size)
                            .FirstOrDefaultAsync(v => v.Id == model.variationId);

            if (variation == null || !variation.isDeleted)
                return new ProductVariationResponseModel { message = "Variation not found or already active." };

            if (model.quantityInStock < 0)
                return new ProductVariationResponseModel { message = "quantity for productVariaiton in stock can't be less than 0!" };
            // Reactivate the variation
            variation.isDeleted = false;

            // Update stock and SKU if provided
            variation.QuantityInStock = model.quantityInStock;

            if (!string.IsNullOrEmpty(model.sku))
                variation.Sku = model.sku;

            // Optionally update photos if new ones are provided
            if (model.photosFiles.Any() == true)
            {
                var newPhotosPaths = await _transferPhotosToPath.GetPhotosPathAsync(model.photosFiles);
                foreach (var path in newPhotosPaths)
                {
                    if (path.StartsWith("error, "))
                    {
                        foreach (var photoPath in newPhotosPaths)
                        {
                            await _transferPhotosToPath.DeleteFileAsync(photoPath);
                        }
                        return new ProductVariationResponseModel { message = path };
                    }
                    var existingPhoto = await _dbContext.Photos
                        .FirstOrDefaultAsync(p => p.ProductVariationId == variation.Id && p.Path == path);

                    if (existingPhoto == null)
                    {
                        _dbContext.Photos.Add(new Photo { ProductVariationId = variation.Id, Path = path });
                    }
                }
                var oldMainVarPhoto = variation.MainProductVariationPhoto;
                variation.MainProductVariationPhoto = newPhotosPaths[0];
                // delete old mainProdPhoto
                await _transferPhotosToPath.DeleteFileAsync(oldMainVarPhoto);
            }

            // update cart rows if the new quantity in stock is less than the quantity in cart
            var cartRows = await _dbContext.CartRows
            .Where(cr => cr.ProductVariationId == model.variationId && cr.Quantity > model.quantityInStock)
            .ToListAsync();

            if (cartRows.Any())
            {
                foreach (var cartRow in cartRows)
                {
                    cartRow.Quantity = model.quantityInStock;
                }
            }

            await _dbContext.SaveChangesAsync();
            return MapToVariationResponse(variation);
        }


        // this funciton will be for the main bage for the App
        public async Task<IEnumerable<GetProductsWithColorsResponse>> GetProductsWithColorsAsync()
        {
            var prodWithColors = await _dbContext.Products
            .AsNoTracking()
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
                return new AddColorResponseModel { message = "this Color value is already stored!" };

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
        // Sizes [C - R]
        public async Task<List<Size>> getSizes()
        {
            return await _dbContext.Sizes.ToListAsync();
        }
        public async Task<AddSizeResponseModel> addSize(string value)
        {
            if (await _dbContext.Sizes.FirstOrDefaultAsync(c => c.Value == value) != null)
                return new AddSizeResponseModel { message = "this Size value is already stored!" };

            // storing the new size 
            var newSize = new Size
            {
                Value = value
            };
            await _dbContext.Sizes.AddAsync(newSize);
            await _dbContext.SaveChangesAsync();
            return new AddSizeResponseModel { message = "", Id = newSize.Id, Value = newSize.Value };
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
                isDeleted = product.isDeleted,
                Price = product.Price,
                category = new CategoryProductsResponse
                {
                    Id = product.Category.Id,
                    value = product.Category.Value
                },
                Variations = product.ProductVariations
                // .Where(v => v.isDeleted) // Exclude soft-deleted variations
                .Select(MapToVariationResponse)
                .ToList()
            };



        private ProductVariationResponseModel MapToVariationResponse(ProductVariation variation) =>
        new ProductVariationResponseModel
        {
            Id = variation.Id,
            Color = variation.Color?.Value ?? "N/A",
            Size = variation.Size?.Value ?? "N/A",
            isDeleted = variation.isDeleted,
            QuantityInStock = variation.QuantityInStock,
            Sku = variation.Sku,
            mainVariationPhotoPath = variation.MainProductVariationPhoto,
            ImagesPathes = variation.Photos?.Select(p => p.Path).ToList() ?? new List<string>(),
            message = ""
        };
    }
}
