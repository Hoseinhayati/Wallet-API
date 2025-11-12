using MediatR;

namespace DigitalWallet.Wallet.Application.Wallets.Commands.WithdrawMoney;

/// <summary>
/// Command to withdraw money from a wallet
/// </summary>
public record WithdrawMoneyCommand : IRequest<WithdrawMoneyResponse>
{
    public Guid WalletId { get; init; }
    public decimal Amount { get; init; }
    public string Currency { get; init; } = "IRR";
    public string Description { get; init; } = string.Empty;
}

/// <summary>
/// Response for withdraw money command
/// </summary>
public record WithdrawMoneyResponse
{
    public Guid WalletId { get; init; }
    public decimal PreviousBalance { get; init; }
    public decimal NewBalance { get; init; }
    public decimal WithdrawnAmount { get; init; }
    public string Currency { get; init; } = string.Empty;
    public DateTime TransactionDate { get; init; }
    public string Message { get; init; } = string.Empty;
    public bool Success { get; init; }
}
