using DigitalWallet.Wallet.Application.Common.Exceptions;
using DigitalWallet.Wallet.Application.Common.Interfaces;
using DigitalWallet.Wallet.Domain.Exceptions;
using DigitalWallet.Wallet.Domain.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DigitalWallet.Wallet.Application.Wallets.Commands.WithdrawMoney;

public class WithdrawMoneyCommandHandler
    : IRequestHandler<WithdrawMoneyCommand, WithdrawMoneyResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<WithdrawMoneyCommandHandler> _logger;

    public WithdrawMoneyCommandHandler(
        IApplicationDbContext context,
        ILogger<WithdrawMoneyCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<WithdrawMoneyResponse> Handle(
        WithdrawMoneyCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Withdrawing {Amount} {Currency} from wallet {WalletId}",
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

        try
        {
            // Withdraw using domain method
            wallet.Withdraw(amount, request.Description);

            // Save changes
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Successfully withdrew {Amount} from wallet {WalletId}. New balance: {NewBalance}",
                request.Amount,
                request.WalletId,
                wallet.Balance.Amount);

            return new WithdrawMoneyResponse
            {
                WalletId = wallet.Id,
                PreviousBalance = previousBalance,
                NewBalance = wallet.Balance.Amount,
                WithdrawnAmount = request.Amount,
                Currency = request.Currency,
                TransactionDate = DateTime.UtcNow,
                Message = "Money withdrawn successfully",
                Success = true
            };
        }
        catch (WalletDomainException ex)
        {
            _logger.LogWarning(ex, "Failed to withdraw from wallet {WalletId}", request.WalletId);
            throw;
        }
    }
}
