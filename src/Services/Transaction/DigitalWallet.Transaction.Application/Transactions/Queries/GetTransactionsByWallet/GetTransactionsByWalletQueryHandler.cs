using DigitalWallet.Transaction.Application.Common.Interfaces;
using DigitalWallet.Transaction.Application.Transactions.DTOs;
using DigitalWallet.Transaction.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DigitalWallet.Transaction.Application.Transactions.Queries.GetTransactionsByWallet;

public class GetTransactionsByWalletQueryHandler
    : IRequestHandler<GetTransactionsByWalletQuery, List<TransactionDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<GetTransactionsByWalletQueryHandler> _logger;

    public GetTransactionsByWalletQueryHandler(
        IApplicationDbContext context,
        ILogger<GetTransactionsByWalletQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<TransactionDto>> Handle(
        GetTransactionsByWalletQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Getting transactions for wallet {WalletId}",
            request.WalletId);

        var query = _context.Transactions
            .AsNoTracking()
            .Where(t => t.WalletId == request.WalletId);

        // Apply filters
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

        // Order by transaction date (newest first)
        query = query.OrderByDescending(t => t.TransactionDate);

        // Apply pagination
        var transactions = await query
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
            "Retrieved {Count} transactions for wallet {WalletId}",
            transactions.Count,
            request.WalletId);

        return transactions;
    }
}
