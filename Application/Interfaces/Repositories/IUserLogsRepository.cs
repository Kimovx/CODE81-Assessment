using CODE81_Assessment.Domain.Entities;

namespace CODE81_Assessment.Application.Interfaces.Repositories
{
    public interface IUserLogsRepository
    {
        public Task<IEnumerable<UserActivityLog>> GetAllPaginatedAsync(int pageNumber, int pageSize);

        public Task<IEnumerable<UserActivityLog>> GetByUserPaginatedAsync(
            int userId, int pageNumber, int pageSize
        );
    }
}
