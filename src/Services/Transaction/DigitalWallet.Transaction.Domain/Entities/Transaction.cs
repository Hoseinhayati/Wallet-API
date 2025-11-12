using DigitalWallet.Transaction.Domain.Common;
using DigitalWallet.Transaction.Domain.DomainEvents;
using DigitalWallet.Transaction.Domain.Enums;
using DigitalWallet.Transaction.Domain.Exceptions;
using DigitalWallet.Transaction.Domain.ValueObjects;

namespace DigitalWallet.Transaction.Domain.Entities;

/// <summary>
/// Transaction aggregate root
/// این Entity Immutable است - فقط Status قابل تغییر است
/// </summary>
public class Transaction : BaseEntity, IAggregateRoot
{
    public Guid WalletId { get; private set; }
    public string UserId { get; private set; }
    public TransactionType Type { get; private set; }
    public Money Amount { get; private set; }
    public TransactionStatus Status { get; private set; }
    public ReferenceCode ReferenceCode { get; private set; }
    public string Description { get; private set; }
    public DateTime TransactionDate { get; private set; }

    // Optional metadata (JSON string)
    public string? Metadata { get; private set; }

    // Related wallet balance at transaction time
    public decimal BalanceAfterTransaction { get; private set; }

    // Private constructor for EF
    private Transaction() { }

    private Transaction(
        Guid walletId,
        string userId,
        TransactionType type,
        Money amount,
        ReferenceCode referenceCode,
        string description,
        decimal balanceAfterTransaction,
        string? metadata = null)
    {
        WalletId = walletId;
        UserId = userId ?? throw new ArgumentNullException(nameof(userId));
        Type = type;
        Amount = amount ?? throw new ArgumentNullException(nameof(amount));
        ReferenceCode = referenceCode ?? throw new ArgumentNullException(nameof(referenceCode));
        Description = description ?? string.Empty;
        BalanceAfterTransaction = balanceAfterTransaction;
        Metadata = metadata;

        Status = TransactionStatus.Completed;
        TransactionDate = DateTime.UtcNow;
    }

    /// <summary>
    /// Factory method to create a new transaction
    /// </summary>
    public static Transaction Create(
        Guid walletId,
        string userId,
        TransactionType type,
        Money amount,
        string description,
        decimal balanceAfterTransaction,
        string? metadata = null)
    {
        // Validation
        if (walletId == Guid.Empty)
            throw new TransactionDomainException("Wallet ID cannot be empty");

        if (string.IsNullOrWhiteSpace(userId))
            throw new TransactionDomainException("User ID cannot be empty");

        if (amount.Amount <= 0)
            throw new TransactionDomainException("Transaction amount must be positive");

        if (balanceAfterTransaction < 0)
            throw new TransactionDomainException("Balance after transaction cannot be negative");

        var referenceCode = ReferenceCode.Generate();

        var transaction = new Transaction(
            walletId,
            userId,
            type,
            amount,
            referenceCode,
            description,
            balanceAfterTransaction,
            metadata
        );

        // Raise domain event
        transaction.AddDomainEvent(new TransactionCreatedEvent(
            transaction.Id,
            transaction.WalletId,
            transaction.UserId,
            transaction.Type,
            transaction.Amount.Amount,
            transaction.Amount.Currency,
            transaction.ReferenceCode.Value,
            transaction.TransactionDate
        ));

        return transaction;
    }

    /// <summary>
    /// Mark transaction as failed
    /// </summary>
    public void MarkAsFailed(string reason)
    {
        if (Status == TransactionStatus.Completed)
            throw new TransactionDomainException("Cannot mark completed transaction as failed");

        Status = TransactionStatus.Failed;
        Description = $"{Description} | Failed: {reason}";

        AddDomainEvent(new TransactionFailedEvent(
            Id,
            WalletId,
            reason
        ));
    }

    /// <summary>
    /// Mark transaction as cancelled
    /// </summary>
    public void Cancel(string reason)
    {
        if (Status == TransactionStatus.Completed)
            throw new TransactionDomainException("Cannot cancel completed transaction");

        Status = TransactionStatus.Cancelled;
        Description = $"{Description} | Cancelled: {reason}";

        AddDomainEvent(new TransactionCancelledEvent(
            Id,
            WalletId,
            reason
        ));
    }
}
