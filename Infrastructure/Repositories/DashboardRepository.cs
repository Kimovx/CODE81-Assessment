using CODE81_Assessment.Application.DTOs.Dashboard;
using CODE81_Assessment.Application.Interfaces.Repositories;
using CODE81_Assessment.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace CODE81_Assessment.Infrastructure.Repositories
{
    public class DashboardRepository(AppDbContext context) : IDashboardRepository
    {
        private readonly AppDbContext _context = context;

        #region KPIs
        public async Task<DashboardKpisDto> GetKpisAsync()
        {
            var totalBooks = await _context.Books.CountAsync();
            var availableBooks = await _context.Books.CountAsync(b => b.Status == BookStatus.Available);
            var borrowedBooks = await _context.Books.CountAsync(b => b.Status == BookStatus.Borrowed);

            var totalMembers = await _context.LibraryMembers.CountAsync();
            var activeMembers = await _context.LibraryMembers.CountAsync(m => m.Status == MemberStatus.Active);
            var suspendedMembers = await _context.LibraryMembers.CountAsync(m => m.Status == MemberStatus.Suspended);
            var expiredMembers = await _context.LibraryMembers.CountAsync(m => m.Status == MemberStatus.Expired);

            var totalTx = await _context.BorrowingTransactions.CountAsync();
            var activeTx = await _context.BorrowingTransactions.CountAsync(t => t.Status == TransactionStatus.Active);
            var returnedTx = await _context.BorrowingTransactions.CountAsync(t => t.Status == TransactionStatus.Returned);
            var overdueTx = await _context.BorrowingTransactions.CountAsync(t => t.Status == TransactionStatus.Overdue);
            var lostTx = await _context.BorrowingTransactions.CountAsync(t => t.Status == TransactionStatus.Lost);

            var totalAuthors = await _context.Authors.CountAsync();
            var totalCategories = await _context.Categories.CountAsync();
            var totalPublishers = await _context.Publishers.CountAsync();

            return new DashboardKpisDto
            {
                TotalBooks = totalBooks,
                AvailableBooks = availableBooks,
                BorrowedBooks = borrowedBooks,

                TotalMembers = totalMembers,
                ActiveMembers = activeMembers,
                SuspendedMembers = suspendedMembers,
                ExpiredMembers = expiredMembers,

                TotalTransactions = totalTx,
                ActiveTransactions = activeTx,
                ReturnedTransactions = returnedTx,
                OverdueTransactions = overdueTx,
                LostTransactions = lostTx,

                TotalAuthors = totalAuthors,
                TotalCategories = totalCategories,
                TotalPublishers = totalPublishers,
            };
        }

        #endregion

        #region Monthly activity

        public async Task<IEnumerable<MonthlyActivityDto>> GetMonthlyActivityAsync(int months)
        {
            var from = DateTimeOffset.UtcNow.AddMonths(-months + 1);
            var cutoff = new DateTimeOffset(from.Year, from.Month, 1, 0, 0, 0, TimeSpan.Zero);

            var transactions = await _context.BorrowingTransactions
                .AsNoTracking()
                .Where(t => t.BorrowDate >= cutoff)
                .Select(t => new
                {
                    t.BorrowDate,
                    t.ReturnDate,
                    t.Status
                })
                .ToListAsync();

            // Build a month-keyed result for the last N months
            var result = Enumerable.Range(0, months)
                .Select(i =>
                {
                    var date = DateTimeOffset.UtcNow.AddMonths(-months + 1 + i);
                    var key = $"{date.Year}-{date.Month:D2}";
                    var month = date.Month;
                    var year = date.Year;

                    var monthTx = transactions
                        .Where(t => t.BorrowDate.Year == year && t.BorrowDate.Month == month)
                        .ToList();

                    return new MonthlyActivityDto
                    {
                        Month = key,
                        BorrowCount = monthTx.Count,
                        ReturnCount = monthTx.Count(t => t.ReturnDate.HasValue),
                        OverdueCount = monthTx.Count(t => t.Status == TransactionStatus.Overdue),
                    };
                })
                .ToList();

            return result;
        }

        #endregion

        #region Top borrowed books
        public async Task<IEnumerable<TopBorrowedBookDto>> GetTopBorrowedBooksAsync(int top)
        {
            return await _context.BorrowingTransactions
                .AsNoTracking()
                .GroupBy(t => t.BookId)
                .Select(g => new
                {
                    BookId = g.Key,
                    BorrowCount = g.Count()
                })
                .OrderByDescending(x => x.BorrowCount)
                .Take(top)
                .Join(
                    _context.Books
                        .Include(b => b.Authors),
                    g => g.BookId,
                    book => book.Id,
                    (g, book) => new TopBorrowedBookDto
                    {
                        BookId = book.Id,
                        Title = book.Title,
                        ISBN = book.ISBN,
                        BorrowCount = g.BorrowCount,
                        Authors = book.Authors.Select(a => a.Name).ToList()
                    })
                .ToListAsync();
        }
        #endregion

        #region Top active members
        public async Task<IEnumerable<TopActiveMemberDto>> GetTopActiveMembersAsync(int top)
        {
            return await _context.BorrowingTransactions
                .AsNoTracking()
                .GroupBy(t => t.MemberId)
                .Select(g => new
                {
                    MemberId = g.Key,
                    BorrowCount = g.Count()
                })
                .OrderByDescending(x => x.BorrowCount)
                .Take(top)
                .Join(
                    _context.LibraryMembers,
                    g => g.MemberId,
                    member => member.Id,
                    (g, member) => new TopActiveMemberDto
                    {
                        MemberId = member.Id,
                        FullName = member.FullName,
                        Email = member.Email,
                        BorrowCount = g.BorrowCount
                    })
                .ToListAsync();
        }
        #endregion

        #region Overdue transactions
        public async Task<IEnumerable<OverdueTransactionDto>> GetOverdueTransactionsAsync()
        {
            var now = DateTime.UtcNow;

            return await _context.BorrowingTransactions
                .AsNoTracking()
                .Where(t => t.Status == TransactionStatus.Active && t.DueDate < now)
                .Include(t => t.Book)
                .Include(t => t.Member)
                .OrderBy(t => t.DueDate)
                .Select(t => new OverdueTransactionDto
                {
                    TransactionId = t.Id,
                    BookId = t.BookId,
                    BookTitle = t.Book.Title,
                    MemberId = t.MemberId,
                    MemberName = t.Member.FullName,
                    MemberEmail = t.Member.Email,
                    BorrowDate = t.BorrowDate,
                    DueDate = t.DueDate,
                    DaysOverdue = (int)(now - t.DueDate).TotalDays
                })
                .ToListAsync();
        }
        #endregion

        #region Recent transactions
        public async Task<IEnumerable<RecentTransactionDto>> GetRecentTransactionsAsync(int top)
        {
            return await _context.BorrowingTransactions
                .AsNoTracking()
                .Include(t => t.Book)
                .Include(t => t.Member)
                .Include(t => t.CreatedBy)
                .OrderByDescending(t => t.BorrowDate)
                .Take(top)
                .Select(t => new RecentTransactionDto
                {
                    TransactionId = t.Id,
                    BookTitle = t.Book.Title,
                    MemberName = t.Member.FullName,
                    Status = t.Status.ToString(),
                    BorrowDate = t.BorrowDate,
                    DueDate = t.DueDate,
                    ReturnDate = t.ReturnDate,
                    CreatedBy = t.CreatedBy.UserName!
                })
                .ToListAsync();
        }
        #endregion
    }
}
