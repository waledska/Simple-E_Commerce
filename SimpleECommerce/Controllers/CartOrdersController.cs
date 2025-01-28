using SimpleECommerce.Services;
using SimpleECommerce.vModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Linq;
using SimpleECommerce.Helpers;
using Microsoft.Extensions.Options;
using SimpleECommerce.DataAndContext;
using Microsoft.EntityFrameworkCore;

namespace SimpleECommerce.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartOrdersController : ControllerBase
    {
        private readonly ICartOrdersService _cartOrdersService;
        private readonly ApplicationDbContext _dbContext;
        public CartOrdersController(ICartOrdersService cartOrdersService, ApplicationDbContext dbContext)
        {
            _cartOrdersService = cartOrdersService;
            _dbContext = dbContext;
        }
        // USER
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost("addItemToMyCart")]
        public async Task<IActionResult> addItemToMyCart([FromBody] addItemToCartModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _cartOrdersService.addItemToMyCartAsync(model);
            if (result != "")
                return BadRequest(result);

            return Ok();
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet("GetMyCartItems")]
        public async Task<IActionResult> GetMyCartItems()
        {
            var result = await _cartOrdersService.getMyCartItemsAsync();

            return Ok(result);
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPut("updateItemQuantityInCart")]
        public async Task<IActionResult> updateItemQuantityInCart([FromBody] addItemToCartModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _cartOrdersService.updateItemQuantityInCartAsync(model);

            if (result != "")
                return BadRequest(result);
            return Ok();
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpDelete("DeleteItemFromCart/{variationId}")]
        public async Task<IActionResult> DeleteItemFromCart(int variationId)
        {
            var result = await _cartOrdersService.DeleteItemFromCartAsync(variationId);
            if (result != "")
                return BadRequest(result);
            return Ok();
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost("cartCheckOut/{addressId}")]
        public async Task<IActionResult> cartCheckOut(int addressId)
        {
            var result = await _cartOrdersService.cartCheckOutAsync(addressId);
            if (result != "")
                return BadRequest(result);
            return Ok();
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost("buyProduct")]
        public async Task<IActionResult> buyProduct([FromBody] buyProdRequestModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _cartOrdersService.buyProdAsync(model);
            if (result != "")
                return BadRequest(result);
            return Ok();
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet("GetMyOrders")]
        public async Task<IActionResult> GetMyOrders()
        {
            var result = await _cartOrdersService.GetMyOrdersAsync();
            return Ok(result);
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet("GetOrderDetails/{orderId}")]
        public async Task<IActionResult> GetOrderDetails(int orderId)
        {
            var result = await _cartOrdersService.GetOrderDetailsAsync(orderId);

            if (result == null)
                return NotFound(new { message = "Order not found" });
            return Ok(result);
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPut("deleteOrderbyUser/{orderId}")]
        public async Task<IActionResult> deleteOrderbyUser(int orderId)
        {
            var result = await _cartOrdersService.deleteOrderbyUserAsync(orderId);

            if (result != "")
                return BadRequest(result);
            return Ok(result);
        }

        [Authorize(AuthenticationSchemes = "Bearer", Roles = "admin")]
        [HttpGet("GetAllUsersOrders")]
        public async Task<IActionResult> GetAllUsersOrders(
        [FromQuery] string? userId = null,
        [FromQuery] string? orderStatus = null,
        [FromQuery] int? orderId = null,
        [FromQuery] string? phoneNumber = null,
        [FromQuery] string? userName = null)
        {
            var result = await _cartOrdersService.GetAllUsersOrdersAsync(userId, orderStatus, orderId, phoneNumber, userName);
            return Ok(result);
        }

        [Authorize(AuthenticationSchemes = "Bearer", Roles = "admin")]
        [HttpPut("updateOrderStatus")]
        public async Task<IActionResult> updateOrderStatus([FromBody] updateOrderStatus model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _cartOrdersService.updateOrderStatusAsync(model);

            if (result != "")
                return BadRequest(result);
            return Ok(result);
        }
    }
}
