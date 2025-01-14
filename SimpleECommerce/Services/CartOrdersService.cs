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
        private readonly orderStatusesData _orderStatusData;

        public CartOrdersService(ApplicationDbContext dbContext
        , IAuthService authService
        , IProdService prodService
        , ILogger<CartOrdersService> logger
        , IOptions<orderStatusesData> orderStatusDataOptions)
        {
            _dbContext = dbContext;
            _authService = authService;
            _prodService = prodService;
            _logger = logger;
            _orderStatusData = orderStatusDataOptions.Value;
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

                    totalAmount += cartItem.ProductVariation.Product.Price * cartItem.Quantity;
                    cartItem.ProductVariation.QuantityInStock -= cartItem.Quantity;
                }

                // creating new order!
                var newOrder = new Order
                {
                    AddressId = addressId,
                    DateOfOrder = DateTime.UtcNow,
                    OrderStatus = _orderStatusData.Pending,
                    TotalAmount = totalAmount
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
                await _dbContext.AddRangeAsync(newOrderRows);

                // empty the cart of the user 
                _dbContext.CartRows.RemoveRange(cartRows);

                // after checkOut after decreasing the amount of prodVars in stocks updates the cart Rows for those prodVars
                //var cartRowsVarIds = cartRows.Select(cr => cr.ProductVariationId).ToList();
                var affectedRowsInUsersCart = await _dbContext.CartRows
                    .Where(cr =>
                        cartRows
                        .Any(c => c.ProductVariationId == cr.ProductVariationId && c.ProductVariation.QuantityInStock < cr.Quantity)
                    )
                    .ToListAsync();

                foreach (var affectedRowInUsersCart in affectedRowsInUsersCart)
                {
                    var matchingCartRow = cartRows.FirstOrDefault(c => c.ProductVariationId == affectedRowInUsersCart.ProductVariationId);
                    if (matchingCartRow != null)
                    {
                        affectedRowInUsersCart.Quantity = matchingCartRow.ProductVariation.QuantityInStock;
                    }
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