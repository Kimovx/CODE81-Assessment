namespace CODE81_Assessment.Application.Interfaces
{
    public interface IUnitOfWork
    {
        // Save Changes
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        // Transactions
        Task BeginTransactionAsync();
        Task CommitAsync();
        Task RollbackAsync();
    }
}
