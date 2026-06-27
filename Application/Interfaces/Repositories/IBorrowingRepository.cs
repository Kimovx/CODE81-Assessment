using CODE81_Assessment.Domain.Entities;

namespace CODE81_Assessment.Application.Interfaces.Repositories
{
    public interface IBorrowingRepository
    {
        #region CRUD
        Task<BorrowingTransaction?> GetByIdAsync(int id, bool asNoTracking = true);

        Task<(IEnumerable<BorrowingTransaction>, int)> GetAllAsync(int page, int size);

        Task<bool> IsBookBorrowedAsync(int bookId);

        Task<int> GetActiveBorrowCountForMember(int memberId);

        Task AddAsync(BorrowingTransaction entity);

        void Update(BorrowingTransaction entity);

        #endregion
    }
}
