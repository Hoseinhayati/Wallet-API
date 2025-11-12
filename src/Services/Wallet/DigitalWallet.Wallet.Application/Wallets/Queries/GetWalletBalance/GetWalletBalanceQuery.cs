using DigitalWallet.Wallet.Application.Wallets.DTOs;
using MediatR;

namespace DigitalWallet.Wallet.Application.Wallets.Queries.GetWalletBalance;

/// <summary>
/// Query to get wallet balance information
/// </summary>
public record GetWalletBalanceQuery : IRequest<WalletBalanceDto>
{
    public Guid WalletId { get; init; }
}
