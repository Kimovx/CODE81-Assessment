using CODE81_Assessment.Application.Interfaces.Repositories;
using CODE81_Assessment.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CODE81_Assessment.Infrastructure.Repositories
{
    public class PublisherRepository(AppDbContext context) : IPublisherRepository
    {
        private readonly AppDbContext _context = context;

        #region CRUD
        public async Task<Publisher?> GetByIdAsync(int id, bool asNoTracking = true)
        {
            var query = _context.Publishers.AsQueryable();

            if (asNoTracking)
                query = query.AsNoTracking();

            return await query.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<(IEnumerable<Publisher>, int)> GetAllAsync(int pageNumber, int pageSize)
        {
            var query = _context.Publishers.AsNoTracking();

            var totalCount = await query.CountAsync();

            var data = await query
                .OrderBy(x => x.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (data, totalCount);
        }

        public async Task<bool> ExistsByNameAsync(string name)
        {
            return await _context.Publishers
                .AsNoTracking()
                .AnyAsync(x => x.Name == name);
        }

        public async Task AddAsync(Publisher publisher)
            => await _context.Publishers.AddAsync(publisher);

        public void Update(Publisher publisher)
            => _context.Publishers.Update(publisher);

        public void Delete(Publisher publisher)
            => publisher.DeletedAt = DateTimeOffset.UtcNow;

        #endregion
    }
}
