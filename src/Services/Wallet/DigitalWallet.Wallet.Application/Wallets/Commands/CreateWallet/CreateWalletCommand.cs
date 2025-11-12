using MediatR;

namespace DigitalWallet.Wallet.Application.Wallets.Commands.CreateWallet;

/// <summary>
/// Command to create a new wallet
/// </summary>
public record CreateWalletCommand : IRequest<CreateWalletResponse>
{
    /// <summary>
    /// User ID (National Code - 10 or 11 digits)
    /// </summary>
    public string UserId { get; init; } = string.Empty;

    /// <summary>
    /// Display name for the wallet
    /// </summary>
    public string DisplayName { get; init; } = string.Empty;

    /// <summary>
    /// Currency (default: IRR)
    /// </summary>
    public string Currency { get; init; } = "IRR";
}

/// <summary>
/// Response for create wallet command
/// </summary>
public record CreateWalletResponse
{
    public Guid WalletId { get; init; }
    public string UserId { get; init; } = string.Empty;
    public string WalletCode { get; init; } = string.Empty;
    public string DisplayName { get; init; } = string.Empty;
    public decimal Balance { get; init; }
    public string Currency { get; init; } = string.Empty;
    public DateTime CreatedDate { get; init; }
    public string Message { get; init; } = string.Empty;
    public bool Success { get; init; }
}
