using MediatR;

namespace DigitalWallet.Wallet.Application.Wallets.Commands.DepositMoney;

/// <summary>
/// Command to deposit money into a wallet
/// </summary>
public record DepositMoneyCommand : IRequest<DepositMoneyResponse>
{
    /// <summary>
    /// Wallet ID to deposit into
    /// </summary>
    public Guid WalletId { get; init; }

    /// <summary>
    /// Amount to deposit
    /// </summary>
    public decimal Amount { get; init; }

    /// <summary>
    /// Currency (default: IRR)
    /// </summary>
    public string Currency { get; init; } = "IRR";

    /// <summary>
    /// Optional description
    /// </summary>
    public string Description { get; init; } = string.Empty;
}

/// <summary>
/// Response for deposit money command
/// </summary>
public record DepositMoneyResponse
{
    public Guid WalletId { get; init; }
    public decimal PreviousBalance { get; init; }
    public decimal NewBalance { get; init; }
    public decimal DepositedAmount { get; init; }
    public string Currency { get; init; } = string.Empty;
    public DateTime TransactionDate { get; init; }
    public string Message { get; init; } = string.Empty;
    public bool Success { get; init; }
}
