using Microsoft.EntityFrameworkCore.Storage;
using CODE81_Assessment.Application.Interfaces;

namespace CODE81_Assessment.Infrastructure
{
    public class UnitOfWork(AppDbContext db) : IUnitOfWork
    {
        private readonly AppDbContext _db = db;
        private IDbContextTransaction? _transaction;

        #region Save Changes
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
            => await _db.SaveChangesAsync(cancellationToken);
        #endregion

        #region Transactions Management

        public async Task BeginTransactionAsync()
        {
            if (_transaction != null)
                return;

            _transaction = await _db.Database.BeginTransactionAsync();
        }

        public async Task CommitAsync()
        {
            if (_transaction is null)
                throw new InvalidOperationException("No active transaction");

            await _transaction.CommitAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }

        public async Task RollbackAsync()
        {
            if (_transaction is null)
                throw new InvalidOperationException("No active transaction");

            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }

        #endregion
    }
}