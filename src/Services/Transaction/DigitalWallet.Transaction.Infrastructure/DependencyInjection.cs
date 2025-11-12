using DigitalWallet.Transaction.Application.Common.Interfaces;
using DigitalWallet.Transaction.Infrastructure.Persistence;
using DigitalWallet.Transaction.Infrastructure.Persistence.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DigitalWallet.Transaction.Infrastructure;

/// <summary>
/// Extension methods for registering infrastructure services
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Register Interceptors
        services.AddScoped<DispatchDomainEventsInterceptor>();

        // Register DbContext
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName));

            // Enable sensitive data logging in development
            options.EnableSensitiveDataLogging();
            options.EnableDetailedErrors();
        });

        // Register IApplicationDbContext
        services.AddScoped<IApplicationDbContext>(provider =>
            provider.GetRequiredService<ApplicationDbContext>());

        // Register Initializer
        services.AddScoped<ApplicationDbContextInitializer>();

        return services;
    }
}
