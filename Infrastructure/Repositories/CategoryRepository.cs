using CODE81_Assessment.Application.Interfaces.Repositories;
using CODE81_Assessment.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CODE81_Assessment.Infrastructure.Repositories
{
    public class CategoryRepository(AppDbContext context) : ICategoryRepository
    {
        private readonly AppDbContext _context = context;

        #region CRUD
        public async Task<Category?> GetByIdAsync(int id)
            => await _context.Categories
                .AsNoTracking()
                .Include(x => x.ParentCategory)
                .FirstOrDefaultAsync(x => x.Id == id);

        public async Task<(IEnumerable<Category>, int totalCount)> GetAllAsync(int pageNumber, int pageSize)
        {
            var query = _context.Categories
                .AsNoTracking()
                .Include(x => x.ParentCategory);

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(x => x.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<bool> ExistsAsync(int id)
            => await _context.Categories.AnyAsync(x => x.Id == id);

        public async Task<List<Category>> GetByIdsAsync(List<int> ids)
            => await _context.Categories
                .Where(x => ids.Contains(x.Id))
                .ToListAsync();

        public async Task<Category?> GetWithSubCategoriesAsync(int id)
            => await _context.Categories
                .Include(c => c.SubCategories)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);

        public async Task AddAsync(Category entity)
            => await _context.Categories.AddAsync(entity);

        public void Update(Category entity)
            => _context.Categories.Update(entity);

        public void Delete(Category entity)
            => entity.DeletedAt = DateTimeOffset.UtcNow;
        #endregion

        #region Other Operations
        public async Task<string> GetCategoryNameAsync(int id)
        {
            var category = await _context.Categories
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);

            return category?.Name ?? string.Empty;
        }
        #endregion
    }
}
