using DigitalWallet.Wallet.Application.Common.Exceptions;
using DigitalWallet.Wallet.Application.Common.Interfaces;
using DigitalWallet.Wallet.Application.Wallets.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DigitalWallet.Wallet.Application.Wallets.Queries.GetWalletBalance;

/// <summary>
/// Handler for GetWalletBalanceQuery
/// </summary>
public class GetWalletBalanceQueryHandler
    : IRequestHandler<GetWalletBalanceQuery, WalletBalanceDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<GetWalletBalanceQueryHandler> _logger;

    public GetWalletBalanceQueryHandler(
        IApplicationDbContext context,
        ILogger<GetWalletBalanceQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<WalletBalanceDto> Handle(
        GetWalletBalanceQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Retrieving balance for wallet {WalletId}",
            request.WalletId);

        var wallet = await _context.Wallets
            .AsNoTracking()
            .FirstOrDefaultAsync(w => w.Id == request.WalletId, cancellationToken);

        if (wallet == null)
        {
            _logger.LogWarning(
                "Wallet {WalletId} not found",
                request.WalletId);

            throw new NotFoundException(nameof(Domain.Entities.Wallet), request.WalletId);
        }

        _logger.LogInformation(
            "Wallet balance retrieved successfully for {WalletId}",
            request.WalletId);

        return new WalletBalanceDto
        {
            WalletId = wallet.Id,
            Code = wallet.Code.Value,
            Balance = wallet.Balance.Amount,
            Currency = wallet.Currency,
            Status = wallet.Status.ToString()
        };
    }
}
