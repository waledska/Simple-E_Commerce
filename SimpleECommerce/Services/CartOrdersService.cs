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
            if (variation == null || variation.isDeleted == true)
                return "invalid variation id, there is no available variations with this id! ";

            if (variation.QuantityInStock < model.quantity)
                return "this amount isn't available in stock for this item!";

            await _dbContext.CartRows.AddAsync(
                new CartRow
                {
                    UserId = currentUserId,
                    ProductVariationId = model.variationId,
                    Quantity = model.quantity
                });
            await _dbContext.SaveChangesAsync();

            return "";
        }

        public async Task<List<CartRow>> getMyCartItemsAsync()
        {
            // you comed back after updating your delete method to soft delete product's variaitons in the user carts!
            var currentUserId = _authService.getUserId();

            var result = await _dbContext.CartRows.Where(cr => cr.UserId == currentUserId)
            //.AsNoTracking()
            .Include(cr => cr.ProductVariation)
            .ThenInclude(pv => pv.Color)
            .Include(cr => cr.ProductVariation)
            .ThenInclude(pv => pv.Size)
            .ToListAsync();

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

        public async Task<string> DeleteItemFromCartAsync(int ItemId)
        {
            var currentUserId = _authService.getUserId();
            var item = await _dbContext.CartRows
                .Where(cr => cr.ProductVariationId == ItemId && cr.UserId == currentUserId)
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
                await _prodService.DeleteVariationForProdAsync(ItemId);
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