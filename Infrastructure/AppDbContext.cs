using CODE81_Assessment.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CODE81_Assessment.Infrastructure
{
    public class AppDbContext(DbContextOptions options) : IdentityDbContext<AppUser, AppRole, int>(options)
    {
        #region Builder
        protected async override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            #region Default Delete
            foreach (var relationship in builder.Model.GetEntityTypes()
             .SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }
            #endregion

            #region Relations
            // Books Relations
            builder.Entity<Book>()
                .HasMany(b => b.Authors)
                .WithMany(a => a.Books)
                .UsingEntity(j => j.ToTable("BooksAuthors"));

            builder.Entity<Book>()
                .HasMany(b => b.Categories)
                .WithMany(c => c.Books)
                .UsingEntity(j => j.ToTable("BooksCategories"));

            builder.Entity<Publisher>()
                .HasMany(p => p.Books)
                .WithOne(b => b.Publisher)
                .HasForeignKey(b => b.PublisherId);

            builder.Entity<Category>()
                .HasOne(c => c.ParentCategory)
                .WithMany(c => c.SubCategories)
                .HasForeignKey(c => c.ParentCategoryId);

            // Transactions Relations
            builder.Entity<AppUser>()
                .HasMany(u => u.CreatedTransactions)
                .WithOne(bt => bt.CreatedBy)
                .HasForeignKey(bt => bt.CreatedById);

            builder.Entity<AppUser>()
                .HasMany(u => u.ReturnedTransactions)
                .WithOne(bt => bt.ReturnedBy)
                .HasForeignKey(bt => bt.ReturnedById);
            #endregion

            #region Update App User Default Constrains
            builder.Entity<AppUser>().Property(u => u.UserName).IsRequired();
            #endregion
        }
        #endregion

        #region Entities
        public DbSet<Author> Authors => Set<Author>();
        public DbSet<Book> Books => Set<Book>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Publisher> Publishers => Set<Publisher>();
        public DbSet<LibraryMember> LibraryMembers => Set<LibraryMember>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
        public DbSet<UserLoginLog> UserLoginLogs => Set<UserLoginLog>();
        public DbSet<BorrowingTransaction> BorrowingTransactions => Set<BorrowingTransaction>();
        #endregion
    }
}