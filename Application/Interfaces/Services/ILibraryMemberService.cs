using CODE81_Assessment.Application.Common;
using CODE81_Assessment.Application.DTOs.LibraryMember;

namespace CODE81_Assessment.Application.Interfaces.Services
{
    public interface ILibraryMemberService
    {
        #region CRUD
        Task<LibraryMemberDto> GetByIdAsync(int id);

        Task<PaginatedResult<LibraryMemberDto>> GetAllAsync(int pageNumber, int pageSize);

        Task<LibraryMemberDto> CreateAsync(LibraryMemberCreateDto dto);

        Task UpdateAsync(int id, LibraryMemberUpdateDto dto);

        Task DeleteAsync(int id);
        #endregion
    }
}
