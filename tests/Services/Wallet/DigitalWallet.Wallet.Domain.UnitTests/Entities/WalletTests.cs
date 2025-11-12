using DigitalWallet.Wallet.Domain.Entities;
using DigitalWallet.Wallet.Domain.Enums;
using DigitalWallet.Wallet.Domain.Exceptions;
using DigitalWallet.Wallet.Domain.ValueObjects;

namespace DigitalWallet.Wallet.Domain.UnitTests.Entities;

public class WalletTests
{
    [Fact]
    public void Create_WithValidData_ShouldCreateWallet()
    {
        // Arrange
        var userId = "1234567890";
        var displayName = "My Wallet";
        var currency = "IRR";

        // Act
        var wallet = Domain.Entities.Wallet.Create(userId, displayName, currency);

        // Assert
        wallet.Should().NotBeNull();
        wallet.Id.Should().NotBeEmpty();
        wallet.UserId.Should().Be(userId);
        wallet.DisplayName.Should().Be(displayName);
        wallet.Currency.Should().Be(currency);
        wallet.Balance.Amount.Should().Be(0);
        wallet.Balance.Currency.Should().Be(currency);
        wallet.Status.Should().Be(WalletStatus.Active);
        wallet.Code.Should().NotBeNull();
        wallet.Code.Value.Should().NotBeEmpty();
        wallet.IsActivated.Should().BeTrue();
        wallet.IsDeleted.Should().BeFalse();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("123")] // کوتاه
    [InlineData("123456789012")] // بلند
    public void Create_WithInvalidUserId_ShouldThrowException(string userId)
    {
        // Arrange & Act
        Action act = () => Domain.Entities.Wallet.Create(userId, "Test", "IRR");

        // Assert
        act.Should().Throw<WalletDomainException>();
    }

    [Fact]
    public void Deposit_WithPositiveAmount_ShouldIncreaseBalance()
    {
        // Arrange
        var wallet = Domain.Entities.Wallet.Create("1234567890", "Test", "IRR");
        wallet.ClearDomainEvents(); // Clear creation event
        var depositAmount = Money.Create(100000, "IRR");

        // Act
        wallet.Deposit(depositAmount, "Test deposit");

        // Assert
        wallet.Balance.Amount.Should().Be(100000);
        wallet.DomainEvents.Should().ContainSingle();
    }

    [Fact]
    public void Deposit_WithNegativeAmount_ShouldThrowException()
    {
        // Arrange
        var wallet = Domain.Entities.Wallet.Create("1234567890", "Test", "IRR");

        // Act
        Action act = () => wallet.Deposit(Money.Create(0, "IRR"));

        // Assert
        act.Should().Throw<WalletDomainException>()
            .WithMessage("*must be positive*");
    }

    [Fact]
    public void Deposit_WithDifferentCurrency_ShouldThrowException()
    {
        // Arrange
        var wallet = Domain.Entities.Wallet.Create("1234567890", "Test", "IRR");
        var usdAmount = Money.Create(100, "USD");

        // Act
        Action act = () => wallet.Deposit(usdAmount);

        // Assert
        act.Should().Throw<WalletDomainException>()
            .WithMessage("*Currency mismatch*");
    }

    [Fact]
    public void Withdraw_WithSufficientBalance_ShouldDecreaseBalance()
    {
        // Arrange
        var wallet = Domain.Entities.Wallet.Create("1234567890", "Test", "IRR");
        wallet.Deposit(Money.Create(100000, "IRR"));
        wallet.ClearDomainEvents(); // Clear deposit event

        // Act
        wallet.Withdraw(Money.Create(50000, "IRR"), "Test withdrawal");

        // Assert
        wallet.Balance.Amount.Should().Be(50000);
        wallet.DomainEvents.Should().ContainSingle();
    }

    [Fact]
    public void Withdraw_WithInsufficientBalance_ShouldThrowException()
    {
        // Arrange
        var wallet = Domain.Entities.Wallet.Create("1234567890", "Test", "IRR");
        wallet.Deposit(Money.Create(50000, "IRR"));

        // Act
        Action act = () => wallet.Withdraw(Money.Create(100000, "IRR"));

        // Assert
        act.Should().Throw<WalletDomainException>()
            .WithMessage("*Insufficient balance*");
    }

    [Fact]
    public void Withdraw_ExactBalance_ShouldResultInZeroBalance()
    {
        // Arrange
        var wallet = Domain.Entities.Wallet.Create("1234567890", "Test", "IRR");
        var amount = Money.Create(100000, "IRR");
        wallet.Deposit(amount);

        // Act
        wallet.Withdraw(amount);

        // Assert
        wallet.Balance.Amount.Should().Be(0);
    }
}
