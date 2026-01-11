using Application.Dtos.Auth;
using Application.Services.Interfaces;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FDAcademy.Controllers
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

        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto input)
        {
            var response = await _authService.LoginAsync(input);
            if (response == null)
            {
                return Unauthorized("Invalid email or password");
            }

            return Ok(response);
        }
        [Authorize]
        [HttpGet("RefreshToken")]
        public async Task<IActionResult> RefreshToken([FromBody] string refreshToken)
        {
            var accessToken = await _authService.RefreshToken(refreshToken);
            return Ok(accessToken);
        }

        [Authorize]
        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto input)
        {
            await _authService.ResetPassword(input);
            return Ok("Password has been reset successfully");
        }
    }
}
