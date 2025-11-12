using DigitalWallet.Transaction.Application.Common.Interfaces;
using DigitalWallet.Transaction.Domain.Enums;
using DigitalWallet.Transaction.Domain.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DigitalWallet.Transaction.Application.Transactions.Commands.RecordTransaction;

public class RecordTransactionCommandHandler
    : IRequestHandler<RecordTransactionCommand, RecordTransactionResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<RecordTransactionCommandHandler> _logger;

    public RecordTransactionCommandHandler(
        IApplicationDbContext context,
        ILogger<RecordTransactionCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<RecordTransactionResponse> Handle(
        RecordTransactionCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Recording transaction: Type={Type}, WalletId={WalletId}, Amount={Amount}",
            request.Type,
            request.WalletId,
            request.Amount);

        // Parse transaction type
        if (!Enum.TryParse<TransactionType>(request.Type, true, out var transactionType))
        {
            throw new ArgumentException($"Invalid transaction type: {request.Type}");
        }

        // Create Money value object
        var amount = Money.Create(request.Amount, request.Currency);

        // Create transaction using domain factory method
        var transaction = Domain.Entities.Transaction.Create(
            request.WalletId,
            request.UserId,
            transactionType,
            amount,
            request.Description,
            request.BalanceAfterTransaction,
            request.Metadata
        );

        // Save to database
        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Transaction recorded successfully: Id={TransactionId}, ReferenceCode={ReferenceCode}",
            transaction.Id,
            transaction.ReferenceCode.Value);

        return new RecordTransactionResponse
        {
            TransactionId = transaction.Id,
            ReferenceCode = transaction.ReferenceCode.Value,
            TransactionDate = transaction.TransactionDate,
            Message = "Transaction recorded successfully",
            Success = true
        };
    }
}
