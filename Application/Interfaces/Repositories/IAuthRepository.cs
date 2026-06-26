using CODE81_Assessment.Domain.Entities;

namespace CODE81_Assessment.Application.Interfaces.Repositories
{
    public interface IAuthRepository
    {
        public Task CreateRefreshToken(RefreshToken refreshToken);

        public Task<RefreshToken?> GetRefreshTokenByTokenId(string tokenId);
    }
}
