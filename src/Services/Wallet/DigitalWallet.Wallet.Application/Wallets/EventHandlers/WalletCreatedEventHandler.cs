using DigitalWallet.Wallet.Domain.DomainEvents;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DigitalWallet.Wallet.Application.Wallets.EventHandlers;

public class WalletCreatedEventHandler : INotificationHandler<WalletCreatedEvent>
{
    private readonly ILogger<WalletCreatedEventHandler> _logger;

    public WalletCreatedEventHandler(ILogger<WalletCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(WalletCreatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Domain Event: Wallet Created - WalletId: {WalletId}, UserId: {UserId}, Code: {Code}",
            notification.WalletId,
            notification.UserId,
            notification.WalletCode);

        // TODO: در آینده اینجا می‌توانیم:
        // - SMS/Email Notification بفرستیم
        // - Integration Event publish کنیم
        // - Cache را initialize کنیم

        return Task.CompletedTask;
    }
}
