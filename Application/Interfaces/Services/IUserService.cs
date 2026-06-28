using CODE81_Assessment.Application.Common;
using CODE81_Assessment.Application.DTOs.User;
using CODE81_Assessment.Application.DTOs.UserLogs;

namespace CODE81_Assessment.Application.Interfaces.Services
{
    public interface IUserService
    {
        #region CRUD
        Task<IEnumerable<UserDto>> GetAllAsync();

        Task<PaginatedResult<UserDto>> GetAllPaginatedAsync(int pageNumber, int pageSize);

        Task<UserDto?> GetByIdAsync(int id);

        Task<UserDto> CreateAsync(UserCreateDto dto);

        Task UpdateAsync(int id, UserUpdateDto dto);

        Task DeleteAsync(int id);
        #endregion

        #region Activity Logs
        public Task<PaginatedResult<UserLogDto>> GetUserActivityLogsPaginatedAsync(int userId, int pageNumber, int pageSize);

        public Task<PaginatedResult<UserLogDto>> GetAllLogsPaginatedAsync(int pageNumber, int pageSize);
        #endregion
    }
}
