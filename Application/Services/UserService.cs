using CODE81_Assessment.Application.Common;
using CODE81_Assessment.Application.DTOs.User;
using CODE81_Assessment.Application.DTOs.UserLogs;
using CODE81_Assessment.Application.Exceptions;
using CODE81_Assessment.Application.Interfaces.Repositories;
using CODE81_Assessment.Application.Interfaces.Services;
using CODE81_Assessment.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace CODE81_Assessment.Application.Services
{
    public class UserService(UserManager<AppUser> userManager,
        RoleManager<AppRole> roleManager,
        IUserLogsRepository userLogsRepository) : IUserService
    {
        private readonly UserManager<AppUser> _userManager = userManager;
        private readonly RoleManager<AppRole> _roleManager = roleManager;
        private readonly IUserLogsRepository _userLogsRepository = userLogsRepository;

        #region Public Methods

        public async Task<IEnumerable<UserDto>> GetAllAsync()
        {
            var users = _userManager.Users.ToList();
            var result = new List<UserDto>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                result.Add(MapToUserDto(user, roles.FirstOrDefault()));
            }

            return result;
        }

        public async Task<PaginatedResult<UserDto>> GetAllPaginatedAsync(int pageNumber, int pageSize)
        {
            if (pageNumber <= 0) pageNumber = 1;
            if (pageSize <= 0) pageSize = 10;

            var query = _userManager.Users;

            var totalCount = query.Count();

            var users = query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var result = new List<UserDto>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                result.Add(MapToUserDto(user, roles.FirstOrDefault()));
            }

            return new PaginatedResult<UserDto>
            {
                Items = result,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<UserDto?> GetByIdAsync(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null) return null;

            var roles = await _userManager.GetRolesAsync(user);
            return MapToUserDto(user, roles.FirstOrDefault());
        }

        public async Task<UserDto> CreateAsync(UserCreateDto dto)
        {
            var role = await GetRoleByIdAsync(dto.RoleId);

            var user = MapToEntity(dto);

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
                throw new UserCreationFailedException();

            await _userManager.AddToRoleAsync(user, role.Name!);

            return MapToUserDto(user, role.Name!);
        }

        public async Task UpdateAsync(int id, UserUpdateDto dto)
        {
            var user = await GetUserByIdOrThrowAsync(id);

            MapToExistingEntity(user, dto);

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
                throw new UserUpdateFailedException();

            if (dto.RoleId.HasValue)
            {
                var role = await GetRoleByIdAsync(dto.RoleId.Value);

                var currentRoles = await _userManager.GetRolesAsync(user);

                await _userManager.RemoveFromRolesAsync(user, currentRoles);
                await _userManager.AddToRoleAsync(user, role.Name!);
            }
        }

        public async Task DeleteAsync(int id)
        {
            var user = await GetUserByIdOrThrowAsync(id);

            var result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded)
                throw new UserDeletionFailedException();
        }

        #endregion

        #region Activity Log Methods
        public async Task<PaginatedResult<UserLogDto>> GetUserActivityLogsPaginatedAsync(int userId, int pageNumber, int pageSize)
        {
            var logs = await _userLogsRepository.GetByUserPaginatedAsync(userId, pageNumber, pageSize);
            var logDtos = logs.Select(MapToUserActivityDto).ToList();

            return new PaginatedResult<UserLogDto>
            {
                Items = logDtos,
                TotalCount = logs.Count(),
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<PaginatedResult<UserLogDto>> GetAllLogsPaginatedAsync(int pageNumber, int pageSize)
        {
            var logs = await _userLogsRepository.GetAllPaginatedAsync(pageNumber, pageSize);
            var logDtos = logs.Select(MapToUserActivityDto).ToList();

            return new PaginatedResult<UserLogDto>
            {
                Items = logDtos,
                TotalCount = logs.Count(),
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
        #endregion

        #region Private Helpers
        private static UserDto MapToUserDto(AppUser user, string? role)
        {
            return new UserDto
            {
                Id = user.Id,
                UserName = user.UserName!,
                Email = user.Email!,
                Role = role
            };
        }

        private static UserLogDto MapToUserActivityDto(UserActivityLog log)
        {
            return new UserLogDto
            {
                Id = log.Id,
                LogTime = log.LogTime,
                IsSuccess = log.IsSuccess,
                IpAddress = log.IpAddress,
                UserAgent = log.UserAgent,
                Action = log.Action,
                UserId = log.UserId,
                UserName = log.User?.UserName
            };
        }

        private static AppUser MapToEntity(UserCreateDto dto)
        {
            return new AppUser
            {
                UserName = dto.UserName,
                Email = dto.Email
            };
        }

        private static void MapToExistingEntity(AppUser user, UserUpdateDto dto)
        {
            user.UserName = dto.UserName;
            user.Email = dto.Email;
        }

        private async Task<AppUser> GetUserByIdOrThrowAsync(int id)
        {
            return await _userManager.FindByIdAsync(id.ToString())
                ?? throw new EntityNotFoundException("User not found");
        }

        private async Task<AppRole> GetRoleByIdAsync(int roleId)
        {
            return await _roleManager.FindByIdAsync(roleId.ToString())
                ?? throw new EntityNotFoundException("Role not found");
        }
        #endregion
    }
}