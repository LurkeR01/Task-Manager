using Application.Services;
using Azure.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.DTOs.Token;
using WebApi.DTOs.User;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        
        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromBody] UserDto dto)
        {
            await _authService.RegisterAsync(dto.Username, dto.Email, dto.Password);
            return Ok();
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] UserDto dto)
        {
            (string accessToken, string refreshToken) = await _authService.LoginAsync(dto.Email, dto.Password);
                
            var cookiesOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.Now.AddDays(7)
            };
                
            Response.Cookies.Append("refreshToken", refreshToken, cookiesOptions);
                
            return Ok(new { accessToken });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> LogoutAsync()
        {
            RefreshDto dto = new RefreshDto { RefreshToken = Request.Cookies["refreshToken"] };
            await _authService.LogoutAsync(dto.RefreshToken);
            return Ok();
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshTokenAsync()
        {
            RefreshDto dto = new RefreshDto { RefreshToken = Request.Cookies["refreshToken"] };
            (string newAccess, string newRefresh) = await _authService.RefreshTokenAsync(dto.RefreshToken);
                
            var cookiesOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.Now.AddDays(7)
            };
                
            Response.Cookies.Append("refreshToken", newRefresh, cookiesOptions);
                
            return Ok(new { accessToken = newAccess});
        }
    }
}
