using DigitalWallet.Wallet.Domain.DomainEvents;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DigitalWallet.Wallet.Application.Wallets.EventHandlers;

public class MoneyWithdrawnEventHandler : INotificationHandler<MoneyWithdrawnEvent>
{
    private readonly ILogger<MoneyWithdrawnEventHandler> _logger;

    public MoneyWithdrawnEventHandler(ILogger<MoneyWithdrawnEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(MoneyWithdrawnEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Domain Event: Money Withdrawn - WalletId: {WalletId}, Amount: {Amount}, NewBalance: {NewBalance}",
            notification.WalletId,
            notification.Amount,
            notification.NewBalance);

        return Task.CompletedTask;
    }
}
