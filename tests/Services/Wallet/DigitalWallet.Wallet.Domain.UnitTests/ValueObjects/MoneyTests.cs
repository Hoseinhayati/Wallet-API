using DigitalWallet.Wallet.Domain.ValueObjects;

namespace DigitalWallet.Wallet.Domain.UnitTests.ValueObjects;

public class MoneyTests
{
    [Fact]
    public void Create_WithValidAmount_ShouldCreateMoney()
    {
        // Arrange & Act
        var money = Money.Create(100, "IRR");

        // Assert
        money.Amount.Should().Be(100);
        money.Currency.Should().Be("IRR");
    }

    [Fact]
    public void Create_WithNegativeAmount_ShouldThrowException()
    {
        // Arrange & Act
        Action act = () => Money.Create(-100, "IRR");

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*cannot be negative*");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void Create_WithEmptyCurrency_ShouldThrowException(string currency)
    {
        // Arrange & Act
        Action act = () => Money.Create(100, currency);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Currency cannot be empty*");
    }

    [Fact]
    public void Add_SameCurrency_ShouldReturnCorrectSum()
    {
        // Arrange
        var money1 = Money.Create(100, "IRR");
        var money2 = Money.Create(200, "IRR");

        // Act
        var result = money1.Add(money2);

        // Assert
        result.Amount.Should().Be(300);
        result.Currency.Should().Be("IRR");
    }

    [Fact]
    public void Add_DifferentCurrency_ShouldThrowException()
    {
        // Arrange
        var irr = Money.Create(100, "IRR");
        var usd = Money.Create(100, "USD");

        // Act
        Action act = () => irr.Add(usd);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*different currencies*");
    }

    [Fact]
    public void Subtract_SameCurrency_ShouldReturnCorrectDifference()
    {
        // Arrange
        var money1 = Money.Create(200, "IRR");
        var money2 = Money.Create(100, "IRR");

        // Act
        var result = money1.Subtract(money2);

        // Assert
        result.Amount.Should().Be(100);
        result.Currency.Should().Be("IRR");
    }

    [Fact]
    public void Equals_SameValues_ShouldBeEqual()
    {
        // Arrange
        var money1 = Money.Create(100, "IRR");
        var money2 = Money.Create(100, "IRR");

        // Act & Assert
        money1.Should().Be(money2);
        (money1 == money2).Should().BeTrue();
    }

    [Fact]
    public void Equals_DifferentValues_ShouldNotBeEqual()
    {
        // Arrange
        var money1 = Money.Create(100, "IRR");
        var money2 = Money.Create(200, "IRR");

        // Act & Assert
        money1.Should().NotBe(money2);
        (money1 != money2).Should().BeTrue();
    }
}
