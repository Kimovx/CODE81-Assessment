using CODE81_Assessment.Domain.Entities;

namespace CODE81_Assessment.Application.Interfaces.Repositories
{
    public interface IPublisherRepository
    {
        #region CRUD

        Task<Publisher?> GetByIdAsync(int id, bool asNoTracking = true);

        Task<(IEnumerable<Publisher>, int)> GetAllAsync(int pageNumber, int pageSize);

        Task<bool> ExistsByNameAsync(string name);

        Task AddAsync(Publisher publisher);

        void Update(Publisher publisher);

        void Delete(Publisher publisher);

        #endregion
    }
}
