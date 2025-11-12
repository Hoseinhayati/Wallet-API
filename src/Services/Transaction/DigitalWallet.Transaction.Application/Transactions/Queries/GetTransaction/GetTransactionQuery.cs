using DigitalWallet.Transaction.Application.Transactions.DTOs;
using MediatR;

namespace DigitalWallet.Transaction.Application.Transactions.Queries.GetTransaction;

/// <summary>
/// Query to get transaction by ID
/// </summary>
public record GetTransactionQuery : IRequest<TransactionDto>
{
    public Guid TransactionId { get; init; }
}
