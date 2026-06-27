using CODE81_Assessment.Application.Common;
using CODE81_Assessment.Application.DTOs.Category;

namespace CODE81_Assessment.Application.Interfaces.Services
{
    public interface ICategoryService
    {
        #region CRUD
        Task<CategoryDto> GetByIdAsync(int id);

        Task<PaginatedResult<CategoryDto>> GetAllAsync(int pageNumber, int pageSize);

        Task<CategoryDto> CreateAsync(CategoryCreateDto dto);

        Task UpdateAsync(int id, CategoryUpdateDto dto);

        Task DeleteAsync(int id);
        #endregion
    }
}
