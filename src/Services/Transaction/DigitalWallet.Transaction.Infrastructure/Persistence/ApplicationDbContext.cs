using DigitalWallet.Transaction.Application.Common.Interfaces;
using DigitalWallet.Transaction.Infrastructure.Persistence.Interceptors;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace DigitalWallet.Transaction.Infrastructure.Persistence;

/// <summary>
/// Application database context for Transaction Service
/// </summary>
public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    private readonly DispatchDomainEventsInterceptor _dispatchDomainEventsInterceptor;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        DispatchDomainEventsInterceptor dispatchDomainEventsInterceptor)
        : base(options)
    {
        _dispatchDomainEventsInterceptor = dispatchDomainEventsInterceptor;
    }

    // DbSets
    public DbSet<Domain.Entities.Transaction> Transactions => Set<Domain.Entities.Transaction>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Apply all configurations from current assembly
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Add interceptor for dispatching domain events
        optionsBuilder.AddInterceptors(_dispatchDomainEventsInterceptor);

        base.OnConfiguring(optionsBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Update timestamps
        foreach (var entry in ChangeTracker.Entries<Domain.Common.BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedDate = DateTime.UtcNow;
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedDate = DateTime.UtcNow;
                    break;
            }
        }

        // Domain Events will be dispatched by the Interceptor
        return await base.SaveChangesAsync(cancellationToken);
    }
}
