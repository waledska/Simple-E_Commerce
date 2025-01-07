using SimpleECommerce.vModels;
using SimpleECommerce.DataAndContext;
using SimpleECommerce.DataAndContext.ModelsForEommerce;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using SimpleECommerce.Migrations;

namespace SimpleECommerce.Services
{
    public class CartOrdersService : ICartOrdersService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IAuthService _authService;
        private readonly IProdService _prodService;

        public CartOrdersService(ApplicationDbContext dbContext, IAuthService authService, IProdService prodService)
        {
            _dbContext = dbContext;
            _authService = authService;
            _prodService = prodService;
        }
        // cartRow[C R U D]
        //[forUser => authentication]
        public async Task<string> addItemToMyCartAsync(addItemToCartModel model)
        {
            var currentUserId = _authService.getUserId();
            var variation = await _dbContext.ProductVariations.FirstOrDefaultAsync(v => v.Id == model.variationId);
            if (variation == null || variation.isDeleted)
                return "invalid variation id, there is no available variations with this id! ";

            if (variation.QuantityInStock < model.quantity)
                return "this amount isn't available in stock for this item!";

            var itemInCart = await _dbContext.CartRows
                .FirstOrDefaultAsync(cr => cr.ProductVariationId == model.variationId && cr.UserId == currentUserId);
            if (itemInCart != null && !variation.isDeleted)
            {
                // user added this item before in his cart
                if (itemInCart.Quantity + model.quantity > variation.QuantityInStock)
                    return "this amount isn't available in stock for this item!";
                itemInCart.Quantity += model.quantity;
            }
            else
            {
                // item not in the user cart
                await _dbContext.CartRows.AddAsync(
                new CartRow
                {
                    UserId = currentUserId,
                    ProductVariationId = model.variationId,
                    Quantity = model.quantity
                });
            }

            await _dbContext.SaveChangesAsync();
            return "";
        }

        public async Task<List<cartItemResponse>> getMyCartItemsAsync()
        {
            // you comed back after updating your delete method to soft delete product's variaitons in the user carts!
            var currentUserId = _authService.getUserId();

            var result = await _dbContext.CartRows.Where(cr => cr.UserId == currentUserId)
            .AsNoTracking()
            .Select(cr => new cartItemResponse
            {
                cartRowId = cr.Id,
                QuantityOfVarInCart = cr.Quantity,
                variaitonId = cr.ProductVariationId,
                isVariationDeleted = cr.ProductVariation.isDeleted,
                mainVarPhoto = cr.ProductVariation.MainProductVariationPhoto,
                sizeValue = cr.ProductVariation.Size.Value,
                colorValue = cr.ProductVariation.Color.Value,
                productId = cr.ProductVariation.Product.Id,
                productDescription = cr.ProductVariation.Product.Description ?? "this product without discription",
                productName = cr.ProductVariation.Product.Name,
                productPrice = cr.ProductVariation.Product.Price,
            }).ToListAsync();

            return result;
        }

        public async Task<string> updateItemQuantityInCartAsync(addItemToCartModel model)
        {
            var currentUserId = _authService.getUserId();

            var Item = await _dbContext.CartRows
                .Where(cr => cr.UserId == currentUserId && cr.ProductVariationId == model.variationId)
                .Include(cr => cr.ProductVariation)
                .FirstOrDefaultAsync();
            if (Item == null)
                return "this item isn't available in your cart";
            // check if the new amount available in Stocks
            if (Item.ProductVariation.QuantityInStock < model.quantity)
                return "this amount isn't available in stock for this item!";
            if (Item.ProductVariation.isDeleted)
                return "this item isn't available any more!";

            Item.Quantity = model.quantity;
            await _dbContext.SaveChangesAsync();
            return "";
        }

        public async Task<string> DeleteItemFromCartAsync(int variationId)
        {
            var currentUserId = _authService.getUserId();
            var item = await _dbContext.CartRows
                .Where(cr => cr.ProductVariationId == variationId && cr.UserId == currentUserId)
                .Include(cr => cr.ProductVariation)
                .FirstOrDefaultAsync();

            if (item == null)
                return "this item isn't already in cart!";

            // remove this cart row
            _dbContext.CartRows.Remove(item);
            var rowsAffected = await _dbContext.SaveChangesAsync();

            // after delete checking if this prod deleted by soft delete and the reason was from the cart only so you should after deleting from the cart applying hard delete!
            if (rowsAffected > 0 && item.ProductVariation.isDeleted)
            {
                await _prodService.DeleteVariationForProdAsync(variationId);
            }

            return "";
        }

        // // orders[C R U D]
        // //[ForUser]
        public async Task<string> cartCheckOutAsync(int addressId)
        {
            throw new NotImplementedException();
        }
        public async Task<string> buyProdAsync(buyProdRequestModel model)
        {
            throw new NotImplementedException();
        }

        public async Task<List<orderWithOutDetails>> GetMyOrdersAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<Order> GetOrderDetailsAsync(int orderId)
        {
            throw new NotImplementedException();
        }

        public async Task<string> deleteOrderbyUserAsync(int orderId)
        {
            throw new NotImplementedException();
        }

        //[ForAdmin]
        public async Task<List<orderWithOutDetails>> GetAllUsersOrdersAsync(string userId = null,
                    string orderStatus = null,
                    int? orderId = null,
                    string phoneNumber = null,
                    string userName = null)
        {
            throw new NotImplementedException();
        }
        public async Task<string> updateOrderStatusAsync(updateOrderStatus model)
        {
            throw new NotImplementedException();
        }
        public async Task<string> deleteOrderbyAdminAsync(int orderId)
        {
            throw new NotImplementedException();
        }
    }
}