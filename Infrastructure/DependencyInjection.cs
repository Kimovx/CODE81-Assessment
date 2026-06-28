using CODE81_Assessment.Application.Interfaces;
using CODE81_Assessment.Application.Interfaces.Repositories;
using CODE81_Assessment.Application.Interfaces.Services;
using CODE81_Assessment.Application.Services;
using CODE81_Assessment.Infrastructure.Repositories;
using CODE81_Assessment.Infrastructure.Services;

namespace CODE81_Assessment.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<JwtOptions>(config.GetSection("Jwt"));

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            #region Auth
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<IAuthService, AuthService>();
            #endregion

            #region Books
            services.AddScoped<IBookRepository, BookRepository>();
            services.AddScoped<IBookService, BookService>();
            #endregion

            #region Authors
            services.AddScoped<IAuthorRepository, AuthorRepository>();
            services.AddScoped<IAuthorService, AuthorService>();
            #endregion

            #region Categories
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<ICategoryService, CategoryService>();
            #endregion

            #region File Storage
            services.AddScoped<IFileStorageService, FileStorageService>();
            #endregion

            #region Publishers
            services.AddScoped<IPublisherRepository, PublisherRepository>();
            services.AddScoped<IPublisherService, PublisherService>();
            #endregion

            #region Library Members
            services.AddScoped<ILibraryMemberRepository, LibraryMemberRepository>();
            services.AddScoped<ILibraryMemberService, LibraryMemberService>();
            #endregion

            #region Borrowing Transaction
            services.AddScoped<IBorrowingRepository, BorrowingRepository>();
            services.AddScoped<IBorrowingService, BorrowingService>();
            #endregion

            #region Users
            services.AddScoped<IUserService, UserService>();
            #endregion

            #region Dashboard
            services.AddScoped<IDashboardRepository, DashboardRepository>();
            services.AddScoped<IDashboardService, DashboardService>();
            #endregion

            #region User Logs
            services.AddScoped<IUserLogsRepository, UserLogsRepository>();
            #endregion

            return services;
        }
    }
}
