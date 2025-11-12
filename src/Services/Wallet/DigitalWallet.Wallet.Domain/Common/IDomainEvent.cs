using MediatR;

namespace DigitalWallet.Wallet.Domain.Common;

public interface IDomainEvent : INotification
{
    DateTime OccurredOn { get; }
    Guid EventId { get; }
}

public abstract record DomainEvent : IDomainEvent
{
    public DateTime OccurredOn { get; init; }
    public Guid EventId { get; init; }

    protected DomainEvent()
    {
        OccurredOn = DateTime.UtcNow;
        EventId = Guid.NewGuid();
    }
}
