using SimpleECommerce.Services;
using SimpleECommerce.vModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Linq;
using SimpleECommerce.Helpers;
using Microsoft.Extensions.Options;

namespace SimpleECommerce.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartOrdersController : ControllerBase
    {
        private readonly ICartOrdersService _cartOrdersService;
        // for testing
        private readonly orderStatuses _OrderStatuses;
        public CartOrdersController(ICartOrdersService cartOrdersService, IOptions<orderStatuses> orderStatuses)
        {
            _cartOrdersService = cartOrdersService;
            _OrderStatuses = orderStatuses.Value;
        }
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
        /// for testing!
        [HttpGet("orderStatus")]
        public async Task<IActionResult> orderStatus()
        {
            return Ok("the status from orderStatuses =>" + _OrderStatuses.pending);
        }


    }
}
