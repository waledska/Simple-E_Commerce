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
    public class ProdController : ControllerBase
    {
        private readonly IProdService _prodService;

        public ProdController(IProdService prodService)
        {
            _prodService = prodService;
        }

        // category [C R U D] APIs
        [HttpPost("CreateCategoty")]
        public async Task<IActionResult> CreateCategoty([FromBody] string value)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _prodService.CreateCategoryAsync(value);
            if (result.message != "")
                return BadRequest(result.message);

            return Ok(result);
        }
        [HttpGet("allCategories")]
        public async Task<IActionResult> GetCategories()
        {
            return Ok(await _prodService.GetCategoriesAsync());
        }

        [HttpPut("UpdateCategory")]
        public async Task<IActionResult> UpdateCategory([FromBody] CategoryModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _prodService.UpdateCategoryAsync(model);
            if (result.message != "")
                return BadRequest(result.message);

            return Ok(result);
        }

        [HttpDelete("DeleteCategory")]
        public async Task<IActionResult> DeleteCategory([FromBody] int catId)
        {
            var result = await _prodService.DeleteCategoryAsync(catId);
            if (result != "")
                return BadRequest(result);

            return Ok(result);
        }

        // Product APIs

        [HttpPost("create")]
        public async Task<IActionResult> CreateProduct([FromBody] ProductRequestModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _prodService.CreateProductAsync(model);
            if (result.message != "")
                return BadRequest(result.message);

            return Ok(result);
        }

        [HttpGet("all")]
        public async Task<IActionResult> ShowProducts()
        {
            var products = await _prodService.ShowProductsAsync();
            if (!products.Any())
                return BadRequest("No products found.");
            return Ok(products);
        }

        [HttpPut("update/{productId}")]
        public async Task<IActionResult> UpdateProduct(int productId, [FromBody] ProductRequestModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _prodService.UpdateProductAsync(productId, model);
            if (result.message != "")
                return BadRequest(result.message);

            return result != null ? Ok(result) : BadRequest("Product not found or it is deleted.");
        }

        [HttpDelete("delete/{productId}")]
        public async Task<IActionResult> DeleteProduct(int productId)
        {
            var result = await _prodService.DeleteProductAsync(productId);
            return result ? Ok() : BadRequest("Product not found.");
        }

        // Reactivate Soft-Deleted Product API
        [HttpPut("reActivate/{productId}")]
        public async Task<IActionResult> ReactivateVariation(int productId)
        {
            var result = await _prodService.ReactivateProductAsync(productId);
            if (result != "")
                return BadRequest(result);

            return Ok();
        }

        // Product Variation APIs

        [HttpPost("{productId}/variation/add")]
        public async Task<IActionResult> AddVariationForProd(int productId, [FromForm] ProductVariationRequestModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _prodService.AddVariationForProdAsync(productId, model);
            if (result.message != "")
                return BadRequest(result.message);

            return Ok(result);
        }

        [HttpGet("{productId}/variations")]
        public async Task<IActionResult> ShowVariationsForProduct(int productId)
        {
            var variations = await _prodService.ShowVariationsForProductAsync(productId);
            if (!variations.Any())
                return NotFound("No active variations found for this product.");
            return Ok(variations);
        }

        [HttpPut("variation/update/{variationId}")]
        public async Task<IActionResult> UpdateVariationForProd(int variationId, [FromForm] ProductVariationRequestModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _prodService.UpdateVariationForProdAsync(variationId, model);
            if (result.message != "")
                return BadRequest(result.message);

            return result != null ? Ok(result) : NotFound("Variation not found or it is deleted.");
        }

        [HttpDelete("variation/delete/{variationId}")]
        public async Task<IActionResult> DeleteVariationForProd(int variationId)
        {
            var result = await _prodService.DeleteVariationForProdAsync(variationId);
            return result ? Ok() : BadRequest("Variation not found.");
        }

        // Get Products with Colors API

        [HttpGet("getProductsWithColors")]
        public async Task<IActionResult> GetProductsWithColors()
        {
            try
            {
                var products = await _prodService.GetProductsWithColorsAsync();
                if (!products.Any())
                    return NotFound("No products with active colors found.");

                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // Reactivate Soft-Deleted Variation API
        [HttpPut("variation/reactivate")]
        public async Task<IActionResult> ReactivateVariation([FromForm] reactivateVariationRequestModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _prodService.ReactivateVariationAsync(model);
            if (result.message != "")
                return BadRequest(result.message);

            return Ok(result);
        }

        // colors
        [HttpPost("AddColor")]
        public async Task<IActionResult> AddNewColor([FromBody] string value)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _prodService.addColor(value);
            if (result.message != "")
                return BadRequest(result.message);

            return Ok(result);
        }
        [HttpGet("GetColors")]
        public async Task<IActionResult> getColors()
        {
            var colors = await _prodService.getColors();
            var result = colors.Select(x => new
            {
                Id = x.Id,
                Value = x.Value
            });
            return Ok(result);
        }

        // sizes
        [HttpPost("AddSize")]
        public async Task<IActionResult> AddNewSize([FromBody] string value)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _prodService.addSize(value);
            if (result.message != "")
                return BadRequest(result.message);

            return Ok(result);
        }
        [HttpGet("GetSizes")]
        public async Task<IActionResult> getSizes()
        {
            var sizes = await _prodService.getSizes();
            var result = sizes.Select(x => new
            {
                Id = x.Id,
                Value = x.Value
            });
            return Ok(result);
        }
    }
}
