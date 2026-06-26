using Microsoft.AspNetCore.Identity;

namespace CODE81_Assessment.Domain.Entities
{
    public class AppUser : IdentityUser<int>
    {
        // Navigations
        public ICollection<BorrowingTransaction> CreatedTransactions { get; set; } = [];
        public ICollection<BorrowingTransaction> ReturnedTransactions { get; set; } = [];
    }
}
