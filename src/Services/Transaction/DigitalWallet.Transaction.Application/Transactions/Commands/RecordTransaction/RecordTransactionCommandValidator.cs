using FluentValidation;

namespace DigitalWallet.Transaction.Application.Transactions.Commands.RecordTransaction;

public class RecordTransactionCommandValidator : AbstractValidator<RecordTransactionCommand>
{
    public RecordTransactionCommandValidator()
    {
        RuleFor(x => x.WalletId)
            .NotEmpty().WithMessage("Wallet ID is required");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required")
            .Length(10, 11).WithMessage("User ID must be 10 or 11 characters");

        RuleFor(x => x.Type)
            .NotEmpty().WithMessage("Transaction type is required")
            .Must(BeValidTransactionType)
            .WithMessage("Transaction type must be: Deposit, Withdrawal, Transfer, Refund, or Fee");

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Amount must be greater than zero")
            .LessThanOrEqualTo(10_000_000_000).WithMessage("Amount cannot exceed 10,000,000,000");

        RuleFor(x => x.Currency)
            .NotEmpty().WithMessage("Currency is required")
            .Must(c => c == "IRR" || c == "USD" || c == "EUR")
            .WithMessage("Currency must be IRR, USD, or EUR");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters");

        RuleFor(x => x.BalanceAfterTransaction)
            .GreaterThanOrEqualTo(0).WithMessage("Balance after transaction cannot be negative");
    }

    private bool BeValidTransactionType(string type)
    {
        var validTypes = new[] { "Deposit", "Withdrawal", "Transfer", "Refund", "Fee" };
        return validTypes.Contains(type, StringComparer.OrdinalIgnoreCase);
    }
}
