using SimpleECommerce.DataAndContext.ModelsForEommerce;
using SimpleECommerce.vModels;

namespace SimpleECommerce.Services
{
    public interface ICartOrdersService
    {
        // cartRow[C R U D]
        //[forUser]
        Task<string> addItemToMyCartAsync(addItemToCartModel model);
        Task<List<CartRow>> getMyCartItemsAsync();
        Task<string> updateItemQuantityInCartAsync(addItemToCartModel model);
        Task<string> DeleteItemFromCartAsync(int ItemId);

        // // orders[C R U D]
        // //[ForUser]
        Task<string> cartCheckOutAsync(int addressId);
        Task<string> buyProdAsync(buyProdRequestModel model);
        Task<List<orderWithOutDetails>> GetMyOrdersAsync(); // in the client chould make filterationBy orderStatus
        Task<Order> GetOrderDetailsAsync(int orderId);
        Task<string> deleteOrderbyUserAsync(int orderId); // [In 8 h]

        //[ForAdmin]
        Task<List<orderWithOutDetails>> GetAllUsersOrdersAsync(string userId = null,
                                                                string orderStatus = null,
                                                                int? orderId = null,
                                                                string phoneNumber = null,
                                                                string userName = null);


        Task<string> updateOrderStatusAsync(updateOrderStatus model);
        Task<string> deleteOrderbyAdminAsync(int orderId);
    }
}
