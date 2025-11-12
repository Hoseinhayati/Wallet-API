using DigitalWallet.Transaction.Domain.Common;

namespace DigitalWallet.Transaction.Domain.DomainEvents;

public sealed record TransactionFailedEvent : DomainEvent
{
    public Guid TransactionId { get; init; }
    public Guid WalletId { get; init; }
    public string Reason { get; init; }

    public TransactionFailedEvent(Guid transactionId, Guid walletId, string reason)
    {
        TransactionId = transactionId;
        WalletId = walletId;
        Reason = reason;
    }
}
