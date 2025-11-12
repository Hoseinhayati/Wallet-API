namespace DigitalWallet.Wallet.Application.Wallets.DTOs;

/// <summary>
/// DTO for wallet balance information
/// </summary>
public record WalletBalanceDto
{
    public Guid WalletId { get; init; }
    public string Code { get; init; } = string.Empty;
    public decimal Balance { get; init; }
    public string Currency { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
}
