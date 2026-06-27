using CODE81_Assessment.Application.Common;
using CODE81_Assessment.Application.DTOs.User;

namespace CODE81_Assessment.Application.Interfaces.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllAsync();

        Task<PaginatedResult<UserDto>> GetAllPaginatedAsync(int pageNumber, int pageSize);

        Task<UserDto?> GetByIdAsync(int id);

        Task<UserDto> CreateAsync(UserCreateDto dto);

        Task UpdateAsync(int id, UserUpdateDto dto);

        Task DeleteAsync(int id);
    }
}
