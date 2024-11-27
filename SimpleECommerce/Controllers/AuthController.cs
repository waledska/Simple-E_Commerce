using SimpleECommerce.Services;
using SimpleECommerce.vModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace SimpleECommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // Get User ID
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet("getUserIdFromToken")]
        public IActionResult GetUserId()
        {
            try
            {
                return Ok(_authService.getUserId());
            }
            catch (Exception ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        // Send OTP to Confirm Email
        [HttpPost("sendOtpToConfirmEmail")]
        public async Task<IActionResult> SendOtpToConfirmEmail([FromBody] sendOTPForLoginModel model)
        {
            var result = await _authService.sendOtpToConfirmEmailAsync(model);
            return result == "" ? Ok("OTP sent successfully.") : BadRequest(result);
        }

        // Register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] userDataModel model)
        {
            var result = await _authService.registerAsync(model);
            return result.Message == "" ? Ok(result) : BadRequest(result.Message);
        }

        // Login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] loginModel model)
        {
            var result = await _authService.getTokenAsync(model);
            return result.Message == "" ? Ok(result) : Unauthorized(result.Message);
        }

        // Logout
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            var result = _authService.LogOutAsync();
            return Ok(new { message = result });
        }

        // Send OTP for Reset Password
        [HttpPost("sendOtpForResetPassword")]
        public async Task<IActionResult> SendOtpForResetPassword([FromBody] sendOTPForLoginModel model)
        {
            var result = await _authService.SendOtpForResetPassAsync(model);
            return result == "" ? Ok("OTP sent for password reset.") : BadRequest(result);
        }

        // Reset Password
        [HttpPost("resetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] resetPassModel model)
        {
            var result = await _authService.resetPassAsync(model);
            return result == "" ? Ok("Password reset successfully.") : BadRequest(result);
        }

        // addreses Area:- 
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost("AddAddress")]
        public async Task<IActionResult> AddAddress([FromBody] AddressInputModel model)
        {
            var result = await _authService.AddAddressToUser(model);
            return Ok(result); // Always returns success if address is added
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet("ShowUserAddresses")]
        public async Task<IActionResult> ShowUserAddresses()
        {
            var result = await _authService.ShowUserAddresses();
            return Ok(result); // Always returns success with the list of addresses
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpDelete("DeleteAddress/{addressId}")]
        public async Task<IActionResult> DeleteAddress(int addressId)
        {
            var result = await _authService.DeleteAddressForUser(addressId);

            if (result == "Address not found")
                return NotFound(result);

            return Ok(result);
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPut("UpdateAddress")]
        public async Task<IActionResult> UpdateAddress([FromBody] AddressInputModel model)
        {
            var result = await _authService.UpdateAddress(model);

            if (result == "Address not found")
                return NotFound(result);

            return Ok(result);
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPut("SetAsDefaultAddress/{addressId}")]
        public async Task<IActionResult> SetAsDefaultAddress(int addressId)
        {
            var result = await _authService.SetAsDefaultAddress(addressId);

            if (result == "Address not found or does not belong to the user.")
                return NotFound(result);

            return Ok(result);
        }
    }
}
