using CODE81_Assessment.Domain.Common;
using CODE81_Assessment.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace CODE81_Assessment.Infrastructure
{
    public class AppDbContext(DbContextOptions options) : IdentityDbContext<AppUser, AppRole, int>(options)
    {
        #region Builder
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            #region Soft Delete Global Filter
            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
                {
                    var method = typeof(AppDbContext)
                        .GetMethod(nameof(SetSoftDeleteFilter), BindingFlags.NonPublic | BindingFlags.Static)!
                        .MakeGenericMethod(entityType.ClrType);

                    method.Invoke(null, [builder]);
                }
            }
            #endregion

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
        public DbSet<UserActivityLog> UserActivityLogs => Set<UserActivityLog>();
        public DbSet<BorrowingTransaction> BorrowingTransactions => Set<BorrowingTransaction>();
        #endregion

        #region Helpers
        private static void SetSoftDeleteFilter<T>(ModelBuilder builder)
        where T : BaseEntity
        {
            builder.Entity<T>().HasQueryFilter(e => !e.DeletedAt.HasValue);
        }
        #endregion
    }
}