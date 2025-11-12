using DigitalWallet.Wallet.Application.Common.Exceptions;
using DigitalWallet.Wallet.Application.Common.Interfaces;
using DigitalWallet.Wallet.Domain.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DigitalWallet.Wallet.Application.Wallets.Commands.DepositMoney;

/// <summary>
/// Handler for DepositMoneyCommand
/// </summary>
public class DepositMoneyCommandHandler
    : IRequestHandler<DepositMoneyCommand, DepositMoneyResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<DepositMoneyCommandHandler> _logger;

    public DepositMoneyCommandHandler(
        IApplicationDbContext context,
        ILogger<DepositMoneyCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<DepositMoneyResponse> Handle(
        DepositMoneyCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Depositing {Amount} {Currency} to wallet {WalletId}",
            request.Amount,
            request.Currency,
            request.WalletId);

        // Find wallet
        var wallet = await _context.Wallets
            .FirstOrDefaultAsync(w => w.Id == request.WalletId, cancellationToken);

        if (wallet == null)
        {
            _logger.LogWarning("Wallet not found: {WalletId}", request.WalletId);
            throw new NotFoundException("Wallet", request.WalletId);
        }

        // Store previous balance
        var previousBalance = wallet.Balance.Amount;

        // Create Money value object
        var amount = Money.Create(request.Amount, request.Currency);

        // Deposit using domain method
        wallet.Deposit(amount, request.Description);

        // Save changes (Domain Events will be dispatched here)
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Successfully deposited {Amount} to wallet {WalletId}. New balance: {NewBalance}",
            request.Amount,
            request.WalletId,
            wallet.Balance.Amount);

        return new DepositMoneyResponse
        {
            WalletId = wallet.Id,
            PreviousBalance = previousBalance,
            NewBalance = wallet.Balance.Amount,
            DepositedAmount = request.Amount,
            Currency = request.Currency,
            TransactionDate = DateTime.UtcNow,
            Message = "Money deposited successfully",
            Success = true
        };
    }
}
