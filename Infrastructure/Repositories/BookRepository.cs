using CODE81_Assessment.Application.Interfaces.Repositories;
using CODE81_Assessment.Domain.Entities;
using CODE81_Assessment.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace CODE81_Assessment.Infrastructure.Repositories
{
    public class BookRepository(AppDbContext context) : IBookRepository
    {
        private readonly AppDbContext _context = context;

        #region CRUD
        public async Task<Book?> GetByIdAsync(int id, bool isTracking = false)
        {
            IQueryable<Book> query = _context.Books;

            if (!isTracking)
                query = query.AsNoTracking();

            return await query
                .Include(x => x.Publisher)
                .Include(x => x.Authors)
                .Include(x => x.Categories)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<(IEnumerable<Book>, int totalCount)> GetAllAsync(int pageNumber, int pageSize)
        {
            var query = _context.Books
                .AsNoTracking()
                .Include(x => x.Publisher);

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(x => x.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<Book?> GetForUpdateAsync(int id)
            => await _context.Books
                .Include(x => x.Authors)
                .Include(x => x.Categories)
                .FirstOrDefaultAsync(x => x.Id == id);

        public async Task AddAsync(Book entity)
            => await _context.Books.AddAsync(entity);

        public void Update(Book entity)
            => _context.Books.Update(entity);

        public void Delete(Book entity)
            => entity.DeletedAt = DateTimeOffset.UtcNow;

        #endregion


        #region Other Operations
        public Task<BookStatus> GetBookStatusAsync(int bookId)
            => _context.Books.Where(x => x.Id == bookId)
                .Select(x => x.Status)
                .FirstOrDefaultAsync();

        public async Task<(IEnumerable<Book> Books, int TotalCount)> SearchAsync(
            string? title,
            string? author,
            string? category,
            int pageNumber,
            int pageSize)
        {
            var query = _context.Books
                .AsNoTracking()
                .Include(b => b.Authors)
                .Include(b => b.Publisher)
                .Include(b => b.Categories)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(title))
            {
                query = query.Where(b => b.Title.ToLower().Contains(title.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(author))
            {
                query = query.Where(b => b.Authors.Any(a => a.Name.ToLower().Contains(author.ToLower())));
            }

            if (!string.IsNullOrWhiteSpace(category))
            {
                query = query.Where(b => b.Categories.Any(c => c.Name.ToLower().Contains(category.ToLower())));
            }

            var totalCount = await query.CountAsync();

            var books = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (books, totalCount);
        }

        public async Task<(IEnumerable<Book> Books, int TotalCount)> GetByStatusAsync(
            BookStatus status,
            int pageNumber,
            int pageSize)
        {
            var query = _context.Books
                .AsNoTracking()
                .Include(b => b.Authors)
                .Include(b => b.Categories)
                .Include(b => b.Publisher)
                .Where(b => b.Status == status)
                .AsQueryable();

            var totalCount = await query.CountAsync();

            var books = await query
                .OrderByDescending(b => b.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (books, totalCount);
        }
        #endregion
    }
}