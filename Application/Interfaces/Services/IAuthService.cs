using CODE81_Assessment.Application.DTOs.Auth;

namespace CODE81_Assessment.Application.Interfaces.Services
{
    public interface IAuthService
    {
        public Task<AuthResponseDto> Login(AuthRequestDto request);

        Task<AuthResponseDto> Refresh(string refreshToken);
    }
}
