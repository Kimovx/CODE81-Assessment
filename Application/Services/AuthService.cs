using CODE81_Assessment.Application.DTOs.Auth;
using CODE81_Assessment.Application.Exceptions;
using CODE81_Assessment.Application.Interfaces;
using CODE81_Assessment.Application.Interfaces.Repositories;
using CODE81_Assessment.Application.Interfaces.Services;
using CODE81_Assessment.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace CODE81_Assessment.Application.Services
{
    public class AuthService(UserManager<AppUser> userManager,
        IJwtService jwtService,
        IAuthRepository authRepository,
        IUnitOfWork unitOfWork) : IAuthService
    {
        private readonly UserManager<AppUser> _userManager = userManager;
        private readonly IJwtService _jwtService = jwtService;
        private readonly IAuthRepository _authRepository = authRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        #region Login 
        public async Task<AuthResponseDto> Login(AuthRequestDto request)
        {
            // Check if user exists
            var user = await _userManager.FindByNameAsync(request.Username)
                ?? throw new InvalidCredentialsException();

            // Validate user password
            var isValid = await _userManager.CheckPasswordAsync(user, request.Password);
            if (!isValid)
                throw new InvalidCredentialsException();

            // Get user role
            var roles = await _userManager.GetRolesAsync(user);
            var primaryRole = roles.FirstOrDefault();

            if (string.IsNullOrWhiteSpace(primaryRole))
                throw new UserHasNoRolesException();

            // Generate JWT
            var accessToken = _jwtService.GenerateToken(user, primaryRole);
            var refreshToken = _jwtService.GenerateRefreshToken();

            // Store The Generated Refresh Token Into The DB
            var combinedRefreshToken = await StoreRefreshToken(refreshToken, user);

            // Commit Storing The Token
            await _unitOfWork.SaveChangesAsync();

            return GetAuthResponseDto(accessToken, combinedRefreshToken);
        }
        #endregion

        #region Refresh Token
        public async Task<AuthResponseDto> Refresh(string refreshToken)
        {
            var storedToken = await CheckRefreshToken(refreshToken);

            // Rotation
            storedToken.RevokedAt = DateTimeOffset.UtcNow;

            var user = storedToken.User;

            // Get user role
            var roles = await _userManager.GetRolesAsync(user);
            var primaryRole = roles.FirstOrDefault();

            if (String.IsNullOrWhiteSpace(primaryRole))
                throw new UserHasNoRolesException();

            // Generate new tokens
            var newAccessToken = _jwtService.GenerateToken(user, primaryRole);
            var newRefreshToken = _jwtService.GenerateRefreshToken();

            var combinedRefreshToken = await StoreRefreshToken(newRefreshToken, user);

            await _unitOfWork.SaveChangesAsync();

            return GetAuthResponseDto(newAccessToken, combinedRefreshToken);
        }
        #endregion

        #region Helpers
        private async Task<string> StoreRefreshToken(string token, AppUser user)
        {
            var hashedToken = BCrypt.Net.BCrypt.HashPassword(token);
            var tokenId = Guid.NewGuid().ToString();

            var refreshTokenEntity = new RefreshToken
            {
                TokenHashed = hashedToken,
                TokenId = tokenId,
                UserId = user.Id,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
            };

            await _authRepository.CreateRefreshToken(refreshTokenEntity);

            return $"{tokenId}.{token}";
        }

        private async Task<RefreshToken> CheckRefreshToken(string refreshToken)
        {
            var parts = refreshToken.Split('.');
            if (parts.Length != 2)
                throw new InvalidRefreshTokenException();

            var tokenId = parts[0];
            var tokenValue = parts[1];

            var storedToken = await _authRepository.GetRefreshTokenByTokenId(tokenId)
                ?? throw new InvalidRefreshTokenException();

            if (storedToken.RevokedAt.HasValue)
                throw new RefreshTokenReuseDetectedException();

            if (storedToken.ExpiresAt < DateTime.UtcNow)
                throw new RefreshTokenExpiredException();

            var isValid = BCrypt.Net.BCrypt.Verify(tokenValue, storedToken.TokenHashed);

            if (!isValid)
                throw new InvalidRefreshTokenException();

            return storedToken;
        }

        private static AuthResponseDto GetAuthResponseDto(string accessToken, string refreshToken)
        {
            return new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }
        #endregion 
    }
}
