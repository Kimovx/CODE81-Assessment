using CODE81_Assessment.Application.Interfaces.Repositories;
using CODE81_Assessment.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CODE81_Assessment.Infrastructure.Repositories
{
    public class UserLogsRepository(AppDbContext context) : IUserLogsRepository
    {
        private readonly AppDbContext _context = context;

        public async Task<IEnumerable<UserActivityLog>> GetByUserPaginatedAsync(
            int userId, int pageNumber, int pageSize
        )
            => await _context.UserActivityLogs
                .AsNoTracking()
                .Include(log => log.User)
                .OrderByDescending(log => log.LogTime)
                .Where(log => log.UserId == userId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

        public async Task<IEnumerable<UserActivityLog>> GetAllPaginatedAsync(int pageNumber, int pageSize)
            => await _context.UserActivityLogs
                .AsNoTracking()
                .Include(log => log.User)
                .OrderByDescending(log => log.LogTime)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
    }
}
