using DigitalWallet.Transaction.Application.Common.Exceptions;
using DigitalWallet.Transaction.Application.Common.Interfaces;
using DigitalWallet.Transaction.Application.Transactions.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DigitalWallet.Transaction.Application.Transactions.Queries.GetTransaction;

public class GetTransactionQueryHandler
    : IRequestHandler<GetTransactionQuery, TransactionDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<GetTransactionQueryHandler> _logger;

    public GetTransactionQueryHandler(
        IApplicationDbContext context,
        ILogger<GetTransactionQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<TransactionDto> Handle(
        GetTransactionQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Getting transaction {TransactionId}",
            request.TransactionId);

        var transaction = await _context.Transactions
            .AsNoTracking()
            .Where(t => t.Id == request.TransactionId)
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
            .FirstOrDefaultAsync(cancellationToken);

        if (transaction == null)
        {
            _logger.LogWarning("Transaction not found: {TransactionId}", request.TransactionId);
            throw new NotFoundException("Transaction", request.TransactionId);
        }

        _logger.LogInformation("Transaction retrieved successfully: {TransactionId}", request.TransactionId);

        return transaction;
    }
}
