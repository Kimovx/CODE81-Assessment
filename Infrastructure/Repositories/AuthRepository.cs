using CODE81_Assessment.Application.Interfaces.Repositories;
using CODE81_Assessment.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CODE81_Assessment.Infrastructure.Repositories
{
    public class AuthRepository(AppDbContext db) : IAuthRepository
    {
        private readonly AppDbContext _db = db;

        #region Create Refresh Token
        public async Task CreateRefreshToken(RefreshToken refreshToken)
            => await _db.RefreshTokens.AddAsync(refreshToken);
        #endregion

        #region Get Refresh Token By Token ID
        public async Task<RefreshToken?> GetRefreshTokenByTokenId(string tokenId)
            => await _db.RefreshTokens.Include(rt => rt.User).FirstOrDefaultAsync(rt => rt.TokenId == tokenId);
        #endregion
    }
}
