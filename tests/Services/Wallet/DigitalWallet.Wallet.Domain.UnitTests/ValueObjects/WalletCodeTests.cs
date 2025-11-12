using DigitalWallet.Wallet.Domain.ValueObjects;

namespace DigitalWallet.Wallet.Domain.UnitTests.ValueObjects;

public class WalletCodeTests
{
    [Fact]
    public void Generate_ShouldCreateValidCode()
    {
        // Act
        var code = WalletCode.Generate();

        // Assert
        code.Should().NotBeNull();
        code.Value.Should().NotBeNullOrEmpty();
        code.Value.Length.Should().BeGreaterOrEqualTo(8);
        code.Value.Length.Should().BeLessOrEqualTo(20);
    }

    [Fact]
    public void Create_WithValidValue_ShouldCreateCode()
    {
        // Arrange
        var value = "ABC12345";

        // Act
        var code = WalletCode.Create(value);

        // Assert
        code.Value.Should().Be("ABC12345");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void Create_WithEmptyValue_ShouldThrowException(string value)
    {
        // Act
        Action act = () => WalletCode.Create(value);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData("ABC123")] // کوتاه
    [InlineData("ABC123456789012345678901")] // بلند
    public void Create_WithInvalidLength_ShouldThrowException(string value)
    {
        // Act
        Action act = () => WalletCode.Create(value);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*between 8 and 20 characters*");
    }

    [Fact]
    public void Equals_SameValue_ShouldBeEqual()
    {
        // Arrange
        var code1 = WalletCode.Create("ABC12345");
        var code2 = WalletCode.Create("ABC12345");

        // Act & Assert
        code1.Should().Be(code2);
        (code1 == code2).Should().BeTrue();
    }
}
