using CODE81_Assessment.Domain.Entities;
using CODE81_Assessment.Domain.Enums;

namespace CODE81_Assessment.Application.Interfaces.Repositories
{
    public interface IBookRepository
    {
        #region CRUD
        Task<Book?> GetByIdAsync(int id, bool isTracking = false);

        Task<(IEnumerable<Book>, int totalCount)> GetAllAsync(int pageNumber, int pageSize);

        Task<Book?> GetForUpdateAsync(int id);

        Task AddAsync(Book entity);
        void Update(Book entity);
        void Delete(Book entity);
        #endregion

        #region Other Operations
        public Task<BookStatus> GetBookStatusAsync(int bookId);

        Task<(IEnumerable<Book> Books, int TotalCount)> SearchAsync(
             string? title,
             string? author,
             string? category,
             int pageNumber,
             int pageSize);

        Task<(IEnumerable<Book> Books, int TotalCount)> GetByStatusAsync(
            BookStatus status,
            int pageNumber,
            int pageSize);
        #endregion
    }
}
