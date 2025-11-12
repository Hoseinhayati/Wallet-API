namespace DigitalWallet.Wallet.Application.Wallets.DTOs;

/// <summary>
/// DTO for complete wallet details
/// </summary>
public record WalletDto
{
    public Guid Id { get; init; }
    public string UserId { get; init; } = string.Empty;
    public string Code { get; init; } = string.Empty;
    public string DisplayName { get; init; } = string.Empty;
    public decimal Balance { get; init; }
    public string Currency { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public DateTime CreatedDate { get; init; }
}
