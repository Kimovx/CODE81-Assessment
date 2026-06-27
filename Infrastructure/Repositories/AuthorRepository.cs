using CODE81_Assessment.Application.Interfaces.Repositories;
using CODE81_Assessment.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CODE81_Assessment.Infrastructure.Repositories
{
    public class AuthorRepository(AppDbContext context) : IAuthorRepository
    {
        private readonly AppDbContext _context = context;

        #region CRUD

        public async Task<Author?> GetByIdAsync(int id)
            => await _context.Authors
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

        public async Task<(IEnumerable<Author>, int totalCount)> GetAllAsync(int pageNumber, int pageSize)
        {
            var query = _context.Authors.AsNoTracking();

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<List<Author>> GetByIdsAsync(List<int> ids)
            => await _context.Authors
                .Where(x => ids.Contains(x.Id))
                .ToListAsync();

        public async Task AddAsync(Author entity)
            => await _context.Authors.AddAsync(entity);

        public void Update(Author entity)
            => _context.Authors.Update(entity);

        public void Delete(Author entity)
            => entity.DeletedAt = DateTimeOffset.UtcNow;

        #endregion
    }
}