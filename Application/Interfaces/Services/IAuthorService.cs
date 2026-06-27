using CODE81_Assessment.Application.Common;
using CODE81_Assessment.Application.DTOs.Author;

namespace CODE81_Assessment.Application.Interfaces.Services
{
    public interface IAuthorService
    {
        #region CRUD
        Task<AuthorDto> GetByIdAsync(int id);

        Task<PaginatedResult<AuthorDto>> GetAllAsync(int pageNumber, int pageSize);

        Task<AuthorDto> CreateAsync(AuthorCreateDto dto);

        Task UpdateAsync(int id, AuthorUpdateDto dto);

        Task DeleteAsync(int id);
        #endregion
    }
}
