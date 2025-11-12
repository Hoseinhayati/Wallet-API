using DigitalWallet.Wallet.Domain.Common;
using DigitalWallet.Wallet.Domain.DomainEvents;
using DigitalWallet.Wallet.Domain.Enums;
using DigitalWallet.Wallet.Domain.Exceptions;
using DigitalWallet.Wallet.Domain.ValueObjects;

namespace DigitalWallet.Wallet.Domain.Entities;

public class Wallet : BaseEntity, IAggregateRoot
{
    public string UserId { get; private set; }
    public WalletCode Code { get; private set; }
    public string DisplayName { get; private set; }
    public Money Balance { get; private set; }
    public WalletStatus Status { get; private set; }
    public string Currency { get; private set; }
    public int Version { get; private set; }

    // EF Core constructor
    private Wallet()
    {
        UserId = null!;
        Code = null!;
        DisplayName = null!;
        Balance = null!;
        Currency = null!;
    }

    private Wallet(string userId, WalletCode code, string displayName, string currency = "IRR")
    {
        UserId = userId ?? throw new ArgumentNullException(nameof(userId));
        Code = code ?? throw new ArgumentNullException(nameof(code));
        DisplayName = displayName ?? throw new ArgumentNullException(nameof(displayName));
        Currency = currency;
        Balance = Money.Zero(currency);
        Status = WalletStatus.Active;
        Version = 1;
    }

    public static Wallet Create(string userId, string displayName, string currency = "IRR")
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new WalletDomainException("User ID cannot be empty");

        if (userId.Length < 10 || userId.Length > 11)
            throw new WalletDomainException("Invalid national code format");

        if (string.IsNullOrWhiteSpace(displayName))
            throw new WalletDomainException("Display name cannot be empty");

        var walletCode = WalletCode.Generate();
        var wallet = new Wallet(userId, walletCode, displayName, currency);

        wallet.AddDomainEvent(new WalletCreatedEvent(
            wallet.Id,
            wallet.UserId,
            wallet.Code.Value,
            wallet.Currency
        ));

        return wallet;
    }

    public void Deposit(Money amount, string description = "")
    {
        if (amount == null)
            throw new WalletDomainException("Amount cannot be null");

        if (amount.Amount <= 0)
            throw new WalletDomainException("Deposit amount must be positive");

        if (amount.Currency != Currency)
            throw new WalletDomainException($"Currency mismatch. Wallet currency is {Currency}");

        if (Status != WalletStatus.Active)
            throw new WalletDomainException($"Cannot deposit to wallet with status: {Status}");

        Balance = Balance.Add(amount);
        Version++;

        AddDomainEvent(new MoneyDepositedEvent(
            Id,
            UserId,
            amount.Amount,
            Balance.Amount,
            description
        ));
    }

    public void Withdraw(Money amount, string description = "")
    {
        if (amount == null)
            throw new WalletDomainException("Amount cannot be null");

        if (amount.Amount <= 0)
            throw new WalletDomainException("Withdrawal amount must be positive");

        if (amount.Currency != Currency)
            throw new WalletDomainException($"Currency mismatch. Wallet currency is {Currency}");

        if (Status != WalletStatus.Active)
            throw new WalletDomainException($"Cannot withdraw from wallet with status: {Status}");

        if (!Balance.IsGreaterThanOrEqual(amount))
            throw new WalletDomainException("Insufficient balance");

        Balance = Balance.Subtract(amount);
        Version++;

        AddDomainEvent(new MoneyWithdrawnEvent(
            Id,
            UserId,
            amount.Amount,
            Balance.Amount,
            description
        ));
    }
}
