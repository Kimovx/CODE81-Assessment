using CODE81_Assessment.Application.Interfaces;

namespace CODE81_Assessment.Infrastructure
{
    public class UnitOfWork(AppDbContext db) : IUnitOfWork
    {
        private readonly AppDbContext _db = db;

        #region Save Changes
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
            => await _db.SaveChangesAsync(cancellationToken);
        #endregion

        #region Transactions Management
        public Task BeginTransactionAsync()
        {
            throw new NotImplementedException();
        }

        public Task CommitAsync()
        {
            throw new NotImplementedException();
        }

        public Task RollbackAsync()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
