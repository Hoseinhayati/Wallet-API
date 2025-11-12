using DigitalWallet.Wallet.Application.Common.Exceptions;
using DigitalWallet.Wallet.Application.Common.Interfaces;
using DigitalWallet.Wallet.Application.Wallets.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DigitalWallet.Wallet.Application.Wallets.Queries.GetWalletDetails;

/// <summary>
/// Handler for GetWalletDetailsQuery
/// </summary>
public class GetWalletDetailsQueryHandler
    : IRequestHandler<GetWalletDetailsQuery, WalletDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<GetWalletDetailsQueryHandler> _logger;

    public GetWalletDetailsQueryHandler(
        IApplicationDbContext context,
        ILogger<GetWalletDetailsQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<WalletDto> Handle(
        GetWalletDetailsQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Retrieving details for wallet {WalletId}",
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
            "Wallet details retrieved successfully for {WalletId}",
            request.WalletId);

        return new WalletDto
        {
            Id = wallet.Id,
            UserId = wallet.UserId,
            Code = wallet.Code.Value,
            DisplayName = wallet.DisplayName,
            Balance = wallet.Balance.Amount,
            Currency = wallet.Currency,
            Status = wallet.Status.ToString(),
            CreatedDate = wallet.CreatedDate
        };
    }
}
