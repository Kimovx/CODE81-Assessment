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

            return services;
        }
}
}
