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



    }
}
