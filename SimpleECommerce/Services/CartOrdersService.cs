using SimpleECommerce.vModels;
using SimpleECommerce.DataAndContext;
using SimpleECommerce.DataAndContext.ModelsForEommerce;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using SimpleECommerce.Migrations;
using Microsoft.Extensions.Options;
using SimpleECommerce.Helpers;

namespace SimpleECommerce.Services
{
    public class CartOrdersService : ICartOrdersService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IAuthService _authService;
        private readonly IProdService _prodService;
        private readonly ILogger<CartOrdersService> _logger;
        private readonly orderStatuses _orderStatusData;
        private readonly TimeSpan _cancellationTimeLimit;

        public CartOrdersService(ApplicationDbContext dbContext
        , IAuthService authService
        , IProdService prodService
        , ILogger<CartOrdersService> logger
        , IOptions<orderStatuses> orderStatusDataOptions
        , IOptions<AmountOfTimeUserAbleToCancellOrder> amountOfTimeToCancellOptions)
        {
            _dbContext = dbContext;
            _authService = authService;
            _prodService = prodService;
            _logger = logger;
            _orderStatusData = orderStatusDataOptions.Value;
            _cancellationTimeLimit = System.Xml.XmlConvert.ToTimeSpan(amountOfTimeToCancellOptions.Value.timePeriod);
        }

        // cartRow[C R U D]
        //[forUser => authentication]
        public async Task<string> addItemToMyCartAsync(addItemToCartModel model)
        {
            var currentUserId = _authService.getUserId();
            var variation = await _dbContext.ProductVariations.FirstOrDefaultAsync(v => v.Id == model.variationId);
            if (variation == null || variation.isDeleted)
                return "invalid variation id, there is no available variations with this id! ";

            if (model.quantity <= 0)
                return "invalid quantity, quantity can't be <= 0";

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
                QuantityOfVariaitonInStock = cr.ProductVariation.QuantityInStock,
                isVariationDeleted = cr.ProductVariation.isDeleted,
                mainVarPhoto = cr.ProductVariation.MainProductVariationPhoto,
                sizeValue = cr.ProductVariation.Size.Value,
                colorValue = cr.ProductVariation.Color.Value,
                productId = cr.ProductVariation.Product.Id,
                productDescription = cr.ProductVariation.Product.Description ?? "this product without discription",
                productName = cr.ProductVariation.Product.Name,
                productPrice = cr.ProductVariation.Product.Price
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
            if (model.quantity <= 0)
                return "invalid quantity, quantity can't be <= 0";
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
            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var currentUserId = _authService.getUserId();
                if (await _dbContext.Addresses.FirstOrDefaultAsync(a => a.Id == addressId && a.UserId == currentUserId) == null)
                    return "there is no addresses asigned to current user with this address Id";

                //get current cart data to make some validations 
                var cartRows = await _dbContext.CartRows
                    .Include(cr => cr.ProductVariation)
                        .ThenInclude(pv => pv.Product)
                    .Where(cr => cr.UserId == currentUserId)
                    .ToListAsync();

                if (cartRows == null || !cartRows.Any())
                    return "your Cart is empty.";

                decimal totalAmount = 0.00m;

                // if amount is not available or variation soft deleted
                foreach (var cartItem in cartRows)
                {
                    var productVariation = cartItem.ProductVariation;
                    if (cartItem.Quantity > productVariation.QuantityInStock)
                        return $"Insufficient stock for Product Variation ID: {productVariation.Id}.";
                    if (cartItem.ProductVariation.isDeleted)
                        return $"productVariaiton with ID: {productVariation.Id} isn't available any more!";
                    if (cartItem.Quantity <= 0)
                        return $"productVariaiton with ID: {productVariation.Id} increase it's quantity in cart First!";
                    if (cartItem.Quantity <= 0 || cartItem.ProductVariation.QuantityInStock <= 0)
                        return $"productVariaiton with ID: {productVariation.Id} isn't available in stock!";

                    totalAmount += cartItem.ProductVariation.Product.Price * cartItem.Quantity;
                    cartItem.ProductVariation.QuantityInStock -= cartItem.Quantity;
                }
                // creating new order!
                var newOrder = new Order
                {
                    AddressId = addressId,
                    DateOfOrder = DateTime.UtcNow,
                    OrderStatus = _orderStatusData.pending,
                    TotalAmount = totalAmount,
                    UserId = currentUserId
                };

                await _dbContext.Orders.AddAsync(newOrder);
                // saving new order
                await _dbContext.SaveChangesAsync();

                // insert the new order's rows
                var newOrderRows = cartRows.Select(cr => new OrderRow
                {
                    OrderId = newOrder.Id,
                    ProductVariationId = cr.ProductVariationId,
                    Quantity = cr.Quantity,
                    PriceForProduct = cr.ProductVariation.Product.Price
                }).ToList();

                // insert new order rows
                await _dbContext.OrderRows.AddRangeAsync(newOrderRows);

                // empty the cart of the user 
                _dbContext.CartRows.RemoveRange(cartRows);

                // after checkOut after decreasing the amount of prodVars in stocks updates the cart Rows for those prodVars
                var affectedVariaitonIds = cartRows.Select(cr => cr.ProductVariationId).Distinct().ToList();
                var affectedCartRowsNeedUpdate = await _dbContext.CartRows
                    .Where(cr =>
                        affectedVariaitonIds.Contains(cr.ProductVariationId)
                        && cr.Quantity > cr.ProductVariation.QuantityInStock
                    )
                    .Include(cr => cr.ProductVariation)
                    .ToListAsync();

                foreach (var affectedCartRowNeedUpdate in affectedCartRowsNeedUpdate)
                {
                    affectedCartRowNeedUpdate.Quantity = affectedCartRowNeedUpdate.ProductVariation.QuantityInStock;
                }


                // save => new order rows, empty cart, decrease the quantity in cart Rows for the affected proVariations If needed
                await _dbContext.SaveChangesAsync();

                // Commit the transaction
                await transaction.CommitAsync();

                return "";
            }
            catch (DbUpdateConcurrencyException ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Concurrency conflict occurred while placing the order.");
                return "The order could not be completed due to a concurrency issue. Please try again.";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "failed to save Order!");
                throw;
            }
        }
        public async Task<string> buyProdAsync(buyProdRequestModel model)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var currentUserId = _authService.getUserId();
                if (await _dbContext.Addresses.FirstOrDefaultAsync(a => a.Id == model.addressId && a.UserId == currentUserId) == null)
                    return "there is no addresses asigned to current user with this address Id";

                var variation = await _dbContext.ProductVariations
                    .Include(pv => pv.Product)
                    .FirstOrDefaultAsync(pv => pv.Id == model.variaitonId);
                if (variation == null)
                    return "invalid productVariaiotnId. there is no variations with this ID!";
                if (model.quantity <= 0)
                    return $"quantity can't be equal 0!";

                // if amount is not available or variation soft deleted
                if (model.quantity > variation.QuantityInStock)
                    return $"Insufficient stock for Product Variation ID: {variation.Id}.";
                if (variation.isDeleted)
                    return $"productVariaiton with ID: {variation.Id} isn't available any more!";
                if (variation.QuantityInStock <= 0)
                    return $"productVariaiton with ID: {variation.Id} isn't available in stock!";

                // calc totalPrice and update quantity in stock
                var totalAmount = variation.Product.Price * model.quantity;
                variation.QuantityInStock -= model.quantity;

                // creating new order!
                var newOrder = new Order
                {
                    AddressId = model.addressId,
                    DateOfOrder = DateTime.UtcNow,
                    OrderStatus = _orderStatusData.pending,
                    TotalAmount = totalAmount,
                    UserId = currentUserId
                };

                await _dbContext.Orders.AddAsync(newOrder);
                // saving new order
                await _dbContext.SaveChangesAsync();

                // insert the new order's row
                await _dbContext.OrderRows.AddAsync(new OrderRow
                {
                    OrderId = newOrder.Id,
                    ProductVariationId = model.variaitonId,
                    Quantity = model.quantity,
                    PriceForProduct = variation.Product.Price
                });

                // after checkOut after decreasing the amount of prodVars in stocks updates the cart Rows for those prodVars
                var affectedCartRowsNeedUpdate = await _dbContext.CartRows
                    .Where(cr =>
                        cr.ProductVariationId == model.variaitonId
                        && model.quantity > cr.ProductVariation.QuantityInStock
                    )
                    .Include(cr => cr.ProductVariation)
                    .ToListAsync();

                foreach (var affectedCartRowNeedUpdate in affectedCartRowsNeedUpdate)
                {
                    affectedCartRowNeedUpdate.Quantity = affectedCartRowNeedUpdate.ProductVariation.QuantityInStock;
                }


                // save => new order rows, empty cart, decrease the quantity in cart Rows for the affected proVariations If needed
                await _dbContext.SaveChangesAsync();

                // Commit the transaction
                await transaction.CommitAsync();

                return "";
            }
            catch (DbUpdateConcurrencyException ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Concurrency conflict occurred while placing the order.");
                return "The order could not be completed due to a concurrency issue. Please try again.";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "failed to save Order!");
                throw;
            }
        }

        public async Task<List<orderWithOutDetails>> GetMyOrdersAsync()
        {
            var currentUserId = _authService.getUserId();
            var result = await _dbContext.Orders
                .AsNoTracking()
                .Where(o => o.UserId == currentUserId)
                .OrderByDescending(o => o.DateOfOrder)
                .Select(o => new orderWithOutDetails
                {
                    orderId = o.Id,
                    UserId = o.UserId,
                    DateOfOrder = o.DateOfOrder,
                    OrderAddress = new simpleAddressDetails
                    {
                        addressId = o.Address.Id,
                        city = o.Address.City,
                        country = o.Address.Country,
                        streetName = o.Address.StreetName
                    },
                    OrderStatus = o.OrderStatus,
                    TotalAmount = o.TotalAmount
                })
                .ToListAsync();

            return result;
        }

        public async Task<Order> GetOrderDetailsAsync(int orderId)
        {
            var currentUserId = _authService.getUserId();

            var result = await _dbContext.Orders
                .AsNoTracking()
                .Include(o => o.Address)
                .Include(o => o.OrderRows)
                .FirstOrDefaultAsync(o => o.UserId == currentUserId && o.Id == orderId);

            return result;
        }

        public async Task<string> deleteOrderbyUserAsync(int orderId)
        {
            var currentUserId = _authService.getUserId();
            var order = await _dbContext.Orders.FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == currentUserId);
            if (order == null)
                return "invalid user ID!";
            if (DateTime.UtcNow - order.DateOfOrder > _cancellationTimeLimit)
                return "the time limit for cancelling current order is Finished! you can't cancel only call customer support";

            order.OrderStatus = _orderStatusData.cancelled;
            await _dbContext.SaveChangesAsync();

            return "";
        }

        //[ForAdmin]
        public async Task<List<Order>> GetAllUsersOrdersAsync(string userId = null,
                    string orderStatus = null,
                    int? orderId = null,
                    string phoneNumber = null,
                    string userName = null)
        {
            // base query If all filteration attributes with null values
            var baseQuery = _dbContext.Orders
                .AsNoTracking()
                .Include(o => o.OrderRows)
                .Include(o => o.Address)
                .AsQueryable();

            if (!string.IsNullOrEmpty(userId))
                baseQuery = baseQuery.Where(o => o.UserId == userId);

            if (orderId.HasValue)
                baseQuery = baseQuery.Where(o => o.Id == orderId);

            if (!string.IsNullOrEmpty(orderStatus))
                baseQuery = baseQuery.Where(o => o.OrderStatus == orderStatus);

            if (!string.IsNullOrEmpty(phoneNumber))
                baseQuery = baseQuery.Where(o => o.Address.MobileNumber.Contains(phoneNumber));

            if (!string.IsNullOrEmpty(userName))
            {
                // var get the userId from his name
                var userIds = await _dbContext.Users
                .AsNoTracking()
                .Where(u => u.UserName.Contains(userName))
                .Select(u => u.Id)
                .ToListAsync();
                baseQuery = baseQuery.Where(o => userIds.Contains(o.UserId));
            }

            return await baseQuery
                .OrderByDescending(o => o.DateOfOrder)
                .ToListAsync();
        }
        // you can delete from here by set orderStatus To Cancelled 
        public async Task<string> updateOrderStatusAsync(updateOrderStatus model)
        {
            var order = await _dbContext.Orders.FirstOrDefaultAsync(o => o.Id == model.orderId);
            if (order == null)
                return "invalid order Id there is no orders with this id!";
            if (order.OrderStatus == model.orderStatus)
                return "this status is already assigned for this order!";

            var orderStatus = "";

            switch (model.orderStatus.ToLower())
            {
                case "pending":
                    orderStatus = "Pending";
                    break;
                case "processing":
                    orderStatus = "Processing";
                    break;
                case "shipped":
                    orderStatus = "Shipped";
                    break;
                case "delivered":
                    orderStatus = "Delivered";
                    break;
                case "cancelled":
                    orderStatus = "Cancelled";
                    break;
                case "returned":
                    orderStatus = "Returned";
                    break;
                default:
                    return "Unknown Status";
            }

            // update order statuss
            order.OrderStatus = orderStatus;
            await _dbContext.SaveChangesAsync();

            return "";
        }
    }
}