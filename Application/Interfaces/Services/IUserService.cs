using CODE81_Assessment.Application.DTOs.User;

namespace CODE81_Assessment.Application.Interfaces.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllAsync();
        
        Task<UserDto?> GetByIdAsync(int id);
        
        Task<UserDto> CreateAsync(UserCreateDto dto);
        
        Task UpdateAsync(int id, UserUpdateDto dto);
        
        Task DeleteAsync(int id);
    }
}
