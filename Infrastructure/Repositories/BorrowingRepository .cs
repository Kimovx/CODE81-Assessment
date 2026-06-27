using CODE81_Assessment.Application.Interfaces.Repositories;
using CODE81_Assessment.Domain.Entities;
using CODE81_Assessment.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace CODE81_Assessment.Infrastructure.Repositories
{
    public class BorrowingRepository(AppDbContext context) : IBorrowingRepository
    {
        private readonly AppDbContext _context = context;

        #region CRUD 
        public async Task<BorrowingTransaction?> GetByIdAsync(int id, bool asNoTracking = true)
        {
            var query = _context.BorrowingTransactions
                .Include(x => x.Member)
                .Include(x => x.Book)
                .AsQueryable();

            if (asNoTracking)
                query = query.AsNoTracking();

            return await query.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<(IEnumerable<BorrowingTransaction>, int)> GetAllAsync(int page, int size)
        {
            var query = _context.BorrowingTransactions
                .Include(x => x.Member)
                .Include(x => x.Book)
                .AsNoTracking();

            var total = await query.CountAsync();

            var data = await query
                .OrderByDescending(x => x.BorrowDate)
                .Skip((page - 1) * size)
                .Take(size)
                .ToListAsync();

            return (data, total);
        }

        public async Task<bool> IsBookBorrowedAsync(int bookId)
        {
            return await _context.BorrowingTransactions
                .AnyAsync(x => x.BookId == bookId && x.Status == TransactionStatus.Active);
        }

        public async Task<int> GetActiveBorrowCountForMember(int memberId)
        {
            return await _context.BorrowingTransactions
                .CountAsync(x => x.MemberId == memberId && x.Status == TransactionStatus.Active);
        }


        public async Task AddAsync(BorrowingTransaction entity)
        {
            await _context.BorrowingTransactions.AddAsync(entity);
        }

        public void Update(BorrowingTransaction entity)
        {
            _context.BorrowingTransactions.Update(entity);
        }
        #endregion
    }
}
