using CODE81_Assessment.Application.DTOs.Auth;
using CODE81_Assessment.Application.Exceptions;
using CODE81_Assessment.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace CODE81_Assessment.Presentaion.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        private readonly IAuthService _authService = authService;

        // Login Method
        [HttpPost("")]
        public async Task<IActionResult> Authenticate([FromBody] AuthRequestDto requestDto)
        {
            if (!ModelState.IsValid)
                throw new BadRequestException();

            var result = await _authService.Login(requestDto);

            if (String.IsNullOrEmpty(result.RefreshToken))
                throw new InvalidRefreshTokenException();

            SaveRefreshToken(result.RefreshToken, 7);

            var response = new AuthResponseDto() { AccessToken = result.AccessToken };

            return Ok(response);
        }

        // Refresh Token Method
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh()
        {
            var refreshToken = Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(refreshToken))
                throw new InvalidRefreshTokenException();

            var result = await _authService.Refresh(refreshToken);

            if (result?.RefreshToken == null)
                throw new InvalidRefreshTokenException();

            SaveRefreshToken(result.RefreshToken, 7);

            return Ok(new AuthResponseDto
            {
                AccessToken = result.AccessToken
            });
        }

        #region Helpers
        private void SaveRefreshToken(string refreshToken, int days)
            => Response.Cookies.Append("rt", refreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddDays(days)
            });
        #endregion
    }
}
