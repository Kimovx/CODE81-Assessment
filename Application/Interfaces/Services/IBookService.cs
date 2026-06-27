using CODE81_Assessment.Application.Common;
using CODE81_Assessment.Application.DTOs.Book;

namespace CODE81_Assessment.Application.Interfaces.Services
{
    public interface IBookService
    {
        #region CRUD
        Task<BookDto> GetByIdAsync(int id);

        Task<PaginatedResult<BookDto>> GetAllAsync(int pageNumber, int pageSize);

        Task<BookDto> CreateAsync(BookCreateDto dto);

        Task UpdateAsync(int id, BookUpdateDto dto);

        Task DeleteAsync(int id);
        #endregion

        #region Other Operations
        Task<PaginatedResult<BookDto>> SearchAsync(BookSearchDto dto);

        Task<PaginatedResult<BookDetailsDto>> GetByStatusAsync(BookByStatusDto dto);
        #endregion
    }
}
