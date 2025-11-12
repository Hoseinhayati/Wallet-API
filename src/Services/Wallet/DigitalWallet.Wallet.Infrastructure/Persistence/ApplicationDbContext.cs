using System.Reflection;
using DigitalWallet.Wallet.Application.Common.Interfaces;
using DigitalWallet.Wallet.Domain.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DigitalWallet.Wallet.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    private readonly IMediator _mediator;
    private readonly ILogger<ApplicationDbContext> _logger;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        IMediator mediator,
        ILogger<ApplicationDbContext> logger)
        : base(options)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public DbSet<Domain.Entities.Wallet> Wallets => Set<Domain.Entities.Wallet>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all configurations from current assembly
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Dispatch domain events before saving
        await DispatchDomainEventsAsync();

        return await base.SaveChangesAsync(cancellationToken);
    }

    private async Task DispatchDomainEventsAsync()
    {
        var domainEntities = ChangeTracker
            .Entries<BaseEntity>()
            .Where(x => x.Entity.DomainEvents.Any())
            .ToList();

        var domainEvents = domainEntities
            .SelectMany(x => x.Entity.DomainEvents)
            .ToList();

        if (domainEvents.Any())
        {
            _logger.LogDebug(
                "Dispatching {EventCount} domain events: {EventTypes}",
                domainEvents.Count,
                string.Join(", ", domainEvents.Select(e => e.GetType().Name)));
        }

        domainEntities.ForEach(entity => entity.Entity.ClearDomainEvents());

        foreach (var domainEvent in domainEvents)
        {
            _logger.LogDebug(
                "Publishing domain event: {EventType} with data: {@Event}",
                domainEvent.GetType().Name,
                domainEvent);

            await _mediator.Publish(domainEvent);
        }
    }
}
