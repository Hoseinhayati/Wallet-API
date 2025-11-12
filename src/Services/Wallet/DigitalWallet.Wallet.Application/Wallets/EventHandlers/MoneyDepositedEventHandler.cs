using DigitalWallet.Wallet.Domain.DomainEvents;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DigitalWallet.Wallet.Application.Wallets.EventHandlers;

public class MoneyDepositedEventHandler : INotificationHandler<MoneyDepositedEvent>
{
    private readonly ILogger<MoneyDepositedEventHandler> _logger;

    public MoneyDepositedEventHandler(ILogger<MoneyDepositedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(MoneyDepositedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Domain Event: Money Deposited - WalletId: {WalletId}, Amount: {Amount}, NewBalance: {NewBalance}",
            notification.WalletId,
            notification.Amount,
            notification.NewBalance);

        // TODO: در آینده اینجا می‌توانیم:
        // - SMS/Email Notification بفرستیم
        // - Integration Event publish کنیم
        // - Cache را update کنیم

        return Task.CompletedTask;
    }
}
