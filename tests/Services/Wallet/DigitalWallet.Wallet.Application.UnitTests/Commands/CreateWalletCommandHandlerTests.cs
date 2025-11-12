using DigitalWallet.Wallet.Application.Common.Interfaces;
using DigitalWallet.Wallet.Application.Wallets.Commands.CreateWallet;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DigitalWallet.Wallet.Application.UnitTests.Commands;

public class CreateWalletCommandHandlerTests
{
    private readonly Mock<IApplicationDbContext> _mockContext;
    private readonly Mock<ILogger<CreateWalletCommandHandler>> _mockLogger;
    private readonly Mock<DbSet<Domain.Entities.Wallet>> _mockDbSet;

    public CreateWalletCommandHandlerTests()
    {
        _mockContext = new Mock<IApplicationDbContext>();
        _mockLogger = new Mock<ILogger<CreateWalletCommandHandler>>();
        _mockDbSet = new Mock<DbSet<Domain.Entities.Wallet>>();

        _mockContext.Setup(x => x.Wallets).Returns(_mockDbSet.Object);
        _mockContext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldCreateWallet()
    {
        // Arrange
        var command = new CreateWalletCommand
        {
            UserId = "1234567890",
            DisplayName = "Test Wallet",
            Currency = "IRR"
        };

        var handler = new CreateWalletCommandHandler(_mockContext.Object, _mockLogger.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.WalletId.Should().NotBeEmpty();
        result.WalletCode.Should().NotBeEmpty();
        result.UserId.Should().Be(command.UserId);
        result.DisplayName.Should().Be(command.DisplayName);
        result.Currency.Should().Be(command.Currency);
        result.Balance.Should().Be(0);
        result.Message.Should().Contain("successfully");

        _mockDbSet.Verify(x => x.Add(It.IsAny<Domain.Entities.Wallet>()), Times.Once);
        _mockContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldLogInformation()
    {
        // Arrange
        var command = new CreateWalletCommand
        {
            UserId = "1234567890",
            DisplayName = "Test Wallet",
            Currency = "IRR"
        };

        var handler = new CreateWalletCommandHandler(_mockContext.Object, _mockLogger.Object);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains("Creating wallet")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.AtLeastOnce);
    }

    [Fact]
    public async Task Handle_WithDefaultCurrency_ShouldUseIRR()
    {
        // Arrange
        var command = new CreateWalletCommand
        {
            UserId = "1234567890",
            DisplayName = "Test Wallet"
            // Currency not specified, should default to IRR
        };

        var handler = new CreateWalletCommandHandler(_mockContext.Object, _mockLogger.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Currency.Should().Be("IRR");
    }
}
