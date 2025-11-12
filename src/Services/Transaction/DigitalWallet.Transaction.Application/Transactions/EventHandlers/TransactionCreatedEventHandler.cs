using DigitalWallet.Transaction.Domain.DomainEvents;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DigitalWallet.Transaction.Application.Transactions.EventHandlers;

public class TransactionCreatedEventHandler : INotificationHandler<TransactionCreatedEvent>
{
    private readonly ILogger<TransactionCreatedEventHandler> _logger;

    public TransactionCreatedEventHandler(ILogger<TransactionCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(TransactionCreatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Domain Event: Transaction Created - Id: {TransactionId}, Type: {Type}, Amount: {Amount}",
            notification.TransactionId,
            notification.Type,
            notification.Amount);

        // TODO: در آینده می‌توانیم:
        // - Analytics را update کنیم
        // - Notification بفرستیم
        // - Integration Event publish کنیم

        return Task.CompletedTask;
    }
}
