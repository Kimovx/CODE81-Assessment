using CODE81_Assessment.Domain.Entities;

namespace CODE81_Assessment.Application.Interfaces.Services
{
    public interface IJwtService
    {
        public string GenerateToken(AppUser userId, string role);

        public string GenerateRefreshToken();
    }
}
