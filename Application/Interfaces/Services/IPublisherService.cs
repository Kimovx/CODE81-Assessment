using CODE81_Assessment.Application.Common;
using CODE81_Assessment.Application.DTOs.Publisher;

namespace CODE81_Assessment.Application.Interfaces.Services
{
    public interface IPublisherService
    {
        #region CRUD
        Task<PublisherDto> GetByIdAsync(int id);

        Task<PaginatedResult<PublisherDto>> GetAllAsync(int pageNumber, int pageSize);

        Task<PublisherDto> CreateAsync(PublisherCreateDto dto);

        Task UpdateAsync(int id, PublisherUpdateDto dto);

        Task DeleteAsync(int id);
        #endregion
    }
}
