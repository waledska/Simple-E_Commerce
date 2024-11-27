using SimpleECommerce.DataAndContext.ModelsForEommerce;
using SimpleECommerce.vModels;

namespace SimpleECommerce.Services
{
    public interface ICartOrdersService
    {
        // what you need to make finally!
        /*

        1- You ‘ll remove table Cart
        2- cartRow[C R U D]
            addProdToMyCart({prodVariationId, Quantity})
            getMyCart
            updateProdQuantityInCart
            DeleteProdFromCart

        3- orders[C R U D]
            makeOrder
            GetMyOrders
	        GetOrderDetails
            deleteOrder[In 8 h]
            [Admin]GetAllUsersOrders(filteration by Users/orderStatus/Order/PhoneNumber/UserName)
            [Admin]changeOrderStatus
	        [Admin]deleteOrder

        */
        //////////////
        // cartRow [C U D]


        // GetMyCart

        // OrderRow [C U D]

        // GetMyOrders

        // GetAllOrdersForAdmin


    }
}
