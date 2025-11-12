using DigitalWallet.Wallet.Application.Wallets.DTOs;
using MediatR;

namespace DigitalWallet.Wallet.Application.Wallets.Queries.GetWalletDetails;

/// <summary>
/// Query to get complete wallet details
/// </summary>
public record GetWalletDetailsQuery : IRequest<WalletDto>
{
    public Guid WalletId { get; init; }
}
