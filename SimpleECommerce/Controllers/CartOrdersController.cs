using SimpleECommerce.Services;
using SimpleECommerce.vModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Linq;

namespace SimpleECommerce.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartOrdersController : ControllerBase
    {
        private readonly ICartOrdersService _cartOrdersService;

        public CartOrdersController(ICartOrdersService cartOrdersService)
        {
            _cartOrdersService = cartOrdersService;
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
        // [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPut("updateItemQuantityInCart")]
        public async Task<IActionResult> updateItemQuantityInCart([FromBody] addItemToCartModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _cartOrdersService.updateItemQuantityInCartAsync(model);

            if (result != "")
                return BadRequest(result);
            return Ok();
        }
        // [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpDelete("DeleteItemFromCart/{variationId}")]
        public async Task<IActionResult> DeleteItemFromCart(int variationId)
        {
            var result = await _cartOrdersService.DeleteItemFromCartAsync(variationId);
            if (result != "")
                return BadRequest(result);
            return Ok();
        }


    }
}
