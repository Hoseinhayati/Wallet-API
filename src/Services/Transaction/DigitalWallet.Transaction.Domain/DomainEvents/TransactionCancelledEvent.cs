using DigitalWallet.Transaction.Domain.Common;

namespace DigitalWallet.Transaction.Domain.DomainEvents;

public sealed record TransactionCancelledEvent : DomainEvent
{
    public Guid TransactionId { get; init; }
    public Guid WalletId { get; init; }
    public string Reason { get; init; }

    public TransactionCancelledEvent(Guid transactionId, Guid walletId, string reason)
    {
        TransactionId = transactionId;
        WalletId = walletId;
        Reason = reason;
    }
}
