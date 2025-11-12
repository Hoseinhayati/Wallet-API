using DigitalWallet.Wallet.Domain.Common;

namespace DigitalWallet.Wallet.Domain.DomainEvents;

public sealed record MoneyWithdrawnEvent : DomainEvent
{
    public Guid WalletId { get; init; }
    public string UserId { get; init; }
    public decimal Amount { get; init; }
    public decimal NewBalance { get; init; }
    public string Description { get; init; }

    public MoneyWithdrawnEvent(Guid walletId, string userId, decimal amount, decimal newBalance, string description)
    {
        WalletId = walletId;
        UserId = userId;
        Amount = amount;
        NewBalance = newBalance;
        Description = description;
    }
}
