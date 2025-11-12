using DigitalWallet.Transaction.Domain.Common;
using DigitalWallet.Transaction.Domain.Enums;

namespace DigitalWallet.Transaction.Domain.DomainEvents;

public sealed record TransactionCreatedEvent : DomainEvent
{
    public Guid TransactionId { get; init; }
    public Guid WalletId { get; init; }
    public string UserId { get; init; }
    public TransactionType Type { get; init; }
    public decimal Amount { get; init; }
    public string Currency { get; init; }
    public string ReferenceCode { get; init; }
    public DateTime TransactionDate { get; init; }

    public TransactionCreatedEvent(
        Guid transactionId,
        Guid walletId,
        string userId,
        TransactionType type,
        decimal amount,
        string currency,
        string referenceCode,
        DateTime transactionDate)
    {
        TransactionId = transactionId;
        WalletId = walletId;
        UserId = userId;
        Type = type;
        Amount = amount;
        Currency = currency;
        ReferenceCode = referenceCode;
        TransactionDate = transactionDate;
    }
}
