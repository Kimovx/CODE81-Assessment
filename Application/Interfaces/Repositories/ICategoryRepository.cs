using CODE81_Assessment.Domain.Entities;

namespace CODE81_Assessment.Application.Interfaces.Repositories
{
    public interface ICategoryRepository
    {
        #region CRUD
        Task<Category?> GetByIdAsync(int id);

        Task<(IEnumerable<Category>, int totalCount)> GetAllAsync(int pageNumber, int pageSize);

        Task<Category?> GetWithSubCategoriesAsync(int id);

        Task AddAsync(Category entity);

        void Update(Category entity);

        void Delete(Category entity);

        Task<bool> ExistsAsync(int id);

        Task<List<Category>> GetByIdsAsync(List<int> ids);
        #endregion

        #region Other Opertations
        public Task<string> GetCategoryNameAsync(int id);
        #endregion
    }
}
