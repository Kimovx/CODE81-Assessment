using CODE81_Assessment.Domain.Entities;

namespace CODE81_Assessment.Application.Interfaces.Repositories
{
    public interface ILibraryMemberRepository
    {
        #region CRUD
        Task<LibraryMember?> GetByIdAsync(int id, bool asNoTracking = true);

        Task<(IEnumerable<LibraryMember> Data, int TotalCount)> GetAllAsync(
            int pageNumber,
            int pageSize);

        Task AddAsync(LibraryMember member);

        void Update(LibraryMember member);

        void Delete(LibraryMember member);
        #endregion
    }
}
