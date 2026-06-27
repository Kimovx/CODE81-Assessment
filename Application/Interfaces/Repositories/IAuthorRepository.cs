using CODE81_Assessment.Domain.Entities;

namespace CODE81_Assessment.Application.Interfaces.Repositories
{
    public interface IAuthorRepository
    {
        Task<Author?> GetByIdAsync(int id);

        Task<(IEnumerable<Author>, int totalCount)> GetAllAsync(int pageNumber, int pageSize);

        Task<List<Author>> GetByIdsAsync(List<int> ids);

        Task AddAsync(Author entity);
        void Update(Author entity);
        void Delete(Author entity);
    }
}
