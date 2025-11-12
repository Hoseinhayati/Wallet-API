using DigitalWallet.Wallet.Domain.Common;

namespace DigitalWallet.Wallet.Domain.DomainEvents;

public sealed record WalletCreatedEvent : DomainEvent
{
    public Guid WalletId { get; init; }
    public string UserId { get; init; }
    public string WalletCode { get; init; }
    public string Currency { get; init; }

    public WalletCreatedEvent(Guid walletId, string userId, string walletCode, string currency)
    {
        WalletId = walletId;
        UserId = userId;
        WalletCode = walletCode;
        Currency = currency;
    }
}
