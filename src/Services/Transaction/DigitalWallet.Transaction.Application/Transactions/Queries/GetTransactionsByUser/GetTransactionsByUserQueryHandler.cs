using DigitalWallet.Transaction.Application.Common.Interfaces;
using DigitalWallet.Transaction.Application.Transactions.DTOs;
using DigitalWallet.Transaction.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DigitalWallet.Transaction.Application.Transactions.Queries.GetTransactionsByUser;

public class GetTransactionsByUserQueryHandler
    : IRequestHandler<GetTransactionsByUserQuery, List<TransactionDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<GetTransactionsByUserQueryHandler> _logger;

    public GetTransactionsByUserQueryHandler(
        IApplicationDbContext context,
        ILogger<GetTransactionsByUserQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<TransactionDto>> Handle(
        GetTransactionsByUserQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting transactions for user {UserId}", request.UserId);

        var query = _context.Transactions
            .AsNoTracking()
            .Where(t => t.UserId == request.UserId);

        // Apply same filters as GetTransactionsByWallet
        if (!string.IsNullOrEmpty(request.Type))
        {
            if (Enum.TryParse<TransactionType>(request.Type, true, out var type))
            {
                query = query.Where(t => t.Type == type);
            }
        }

        if (!string.IsNullOrEmpty(request.Status))
        {
            if (Enum.TryParse<TransactionStatus>(request.Status, true, out var status))
            {
                query = query.Where(t => t.Status == status);
            }
        }

        if (request.StartDate.HasValue)
        {
            query = query.Where(t => t.TransactionDate >= request.StartDate.Value);
        }

        if (request.EndDate.HasValue)
        {
            query = query.Where(t => t.TransactionDate <= request.EndDate.Value);
        }

        var transactions = await query
            .OrderByDescending(t => t.TransactionDate)
            .Skip(request.Skip)
            .Take(request.Take)
            .Select(t => new TransactionDto
            {
                Id = t.Id,
                WalletId = t.WalletId,
                UserId = t.UserId,
                Type = t.Type.ToString(),
                Amount = t.Amount.Amount,
                Currency = t.Amount.Currency,
                Status = t.Status.ToString(),
                ReferenceCode = t.ReferenceCode.Value,
                Description = t.Description,
                BalanceAfterTransaction = t.BalanceAfterTransaction,
                TransactionDate = t.TransactionDate,
                CreatedDate = t.CreatedDate
            })
            .ToListAsync(cancellationToken);

        _logger.LogInformation(
            "Retrieved {Count} transactions for user {UserId}",
            transactions.Count,
            request.UserId);

        return transactions;
    }
}
