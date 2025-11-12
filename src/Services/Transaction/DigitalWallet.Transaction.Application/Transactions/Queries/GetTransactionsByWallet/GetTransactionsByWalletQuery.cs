using DigitalWallet.Transaction.Application.Transactions.DTOs;
using MediatR;

namespace DigitalWallet.Transaction.Application.Transactions.Queries.GetTransactionsByWallet;

/// <summary>
/// Query to get all transactions for a wallet
/// </summary>
public record GetTransactionsByWalletQuery : IRequest<List<TransactionDto>>
{
    public Guid WalletId { get; init; }

    /// <summary>
    /// Filter by transaction type (optional)
    /// </summary>
    public string? Type { get; init; }

    /// <summary>
    /// Filter by status (optional)
    /// </summary>
    public string? Status { get; init; }

    /// <summary>
    /// Start date filter (optional)
    /// </summary>
    public DateTime? StartDate { get; init; }

    /// <summary>
    /// End date filter (optional)
    /// </summary>
    public DateTime? EndDate { get; init; }

    /// <summary>
    /// Number of records to take (default: 100)
    /// </summary>
    public int Take { get; init; } = 100;

    /// <summary>
    /// Number of records to skip (for pagination)
    /// </summary>
    public int Skip { get; init; } = 0;
}
