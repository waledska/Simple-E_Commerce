using SimpleECommerce.vModels;
using SimpleECommerce.DataAndContext;
using SimpleECommerce.DataAndContext.ModelsForEommerce;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace SimpleECommerce.Services
{
    public class CartOrdersService : ICartOrdersService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IAuthService _authService;

        public CartOrdersService(ApplicationDbContext dbContext, IAuthService authService)
        {
            _dbContext = dbContext;
            _authService = authService;
        }
        // cartRow[C R U D]
        //[forUser => authentication]
        public async Task<string> addItemToMyCartAsync(addItemToCartModel model)
        {
            var currentUserId = _authService.getUserId();
            var variation = await _dbContext.ProductVariations.FirstOrDefaultAsync(v => v.Id == model.variationId);
            if (variation == null || variation.isDeleted == true)
                return "invalid variation id, there is no variations woth this id! ";

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

        public Task<List<CartRow>> getMyCartItemsAsync()
        {
            // you need update while deleting the proVariaiton as 
            var currentUserId = _authService.getUserId();

            throw new NotImplementedException();
        }

        public Task<string> updateItemQuantityInCartAsync(addItemToCartModel model)
        {
            throw new NotImplementedException();
        }

        public Task<string> DeleteItemFromCartAsync(int ItemId)
        {
            throw new NotImplementedException();
        }

        // // orders[C R U D]
        // //[ForUser]
        public Task<string> cartCheckOutAsync(int addressId)
        {
            throw new NotImplementedException();
        }
        public Task<string> buyProdAsync(buyProdRequestModel model)
        {
            throw new NotImplementedException();
        }

        public Task<List<orderWithOutDetails>> GetMyOrdersAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Order> GetOrderDetailsAsync(int orderId)
        {
            throw new NotImplementedException();
        }

        public Task<string> deleteOrderbyUserAsync(int orderId)
        {
            throw new NotImplementedException();
        }

        //[ForAdmin]
        public Task<List<orderWithOutDetails>> GetAllUsersOrdersAsync(string userId = null,
                    string orderStatus = null,
                    int? orderId = null,
                    string phoneNumber = null,
                    string userName = null)
        {
            throw new NotImplementedException();
        }
        public Task<string> updateOrderStatusAsync(updateOrderStatus model)
        {
            throw new NotImplementedException();
        }
        public Task<string> deleteOrderbyAdminAsync(int orderId)
        {
            throw new NotImplementedException();
        }
    }
}