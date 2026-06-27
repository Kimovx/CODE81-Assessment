using CODE81_Assessment.Application.Common;
using CODE81_Assessment.Application.DTOs.BorrowingTransaction;

namespace CODE81_Assessment.Application.Interfaces.Services
{
    public interface IBorrowingService
    {
        Task<BorrowingTransactionDto> GetByIdAsync(int id);

        Task<PaginatedResult<BorrowingTransactionDto>> GetAllAsync(int page, int size);

        Task<BorrowingTransactionDto> BorrowAsync(BorrowingTransactionCreateDto dto, int userId);

        Task ReturnAsync(ReturnBookDto dto, int userId);
    }
}
