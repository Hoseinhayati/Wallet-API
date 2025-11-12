using DigitalWallet.Transaction.Application.Transactions.DTOs;
using MediatR;

namespace DigitalWallet.Transaction.Application.Transactions.Queries.GetTransactionsByUser;

/// <summary>
/// Query to get all transactions for a user
/// </summary>
public record GetTransactionsByUserQuery : IRequest<List<TransactionDto>>
{
    public string UserId { get; init; } = string.Empty;

    public string? Type { get; init; }
    public string? Status { get; init; }
    public DateTime? StartDate { get; init; }
    public DateTime? EndDate { get; init; }
    public int Take { get; init; } = 100;
    public int Skip { get; init; } = 0;
}
