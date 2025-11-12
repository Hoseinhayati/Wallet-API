using MediatR;

namespace DigitalWallet.Transaction.Application.Transactions.Commands.RecordTransaction;

/// <summary>
/// Command to record a new transaction
/// </summary>
public record RecordTransactionCommand : IRequest<RecordTransactionResponse>
{
    /// <summary>
    /// Wallet ID
    /// </summary>
    public Guid WalletId { get; init; }

    /// <summary>
    /// User ID (National Code)
    /// </summary>
    public string UserId { get; init; } = string.Empty;

    /// <summary>
    /// Transaction type (Deposit, Withdrawal, Transfer, etc.)
    /// </summary>
    public string Type { get; init; } = string.Empty;

    /// <summary>
    /// Transaction amount
    /// </summary>
    public decimal Amount { get; init; }

    /// <summary>
    /// Currency (default: IRR)
    /// </summary>
    public string Currency { get; init; } = "IRR";

    /// <summary>
    /// Transaction description
    /// </summary>
    public string Description { get; init; } = string.Empty;

    /// <summary>
    /// Wallet balance after this transaction
    /// </summary>
    public decimal BalanceAfterTransaction { get; init; }

    /// <summary>
    /// Optional metadata (JSON string)
    /// </summary>
    public string? Metadata { get; init; }
}

/// <summary>
/// Response for record transaction command
/// </summary>
public record RecordTransactionResponse
{
    public Guid TransactionId { get; init; }
    public string ReferenceCode { get; init; } = string.Empty;
    public DateTime TransactionDate { get; init; }
    public string Message { get; init; } = string.Empty;
    public bool Success { get; init; }
}
