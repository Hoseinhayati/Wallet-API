using FluentValidation;

namespace DigitalWallet.Wallet.Application.Wallets.Commands.WithdrawMoney;

public class WithdrawMoneyCommandValidator : AbstractValidator<WithdrawMoneyCommand>
{
    public WithdrawMoneyCommandValidator()
    {
        RuleFor(x => x.WalletId)
            .NotEmpty().WithMessage("شناسه کیف پول الزامی است");

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("مبلغ برداشتی باید بیشتر از صفر باشد")
            .LessThanOrEqualTo(1_000_000_000).WithMessage("حداکثر مبلغ برداشتی ۱٬۰۰۰٬۰۰۰٬۰۰۰ است");

        RuleFor(x => x.Currency)
            .NotEmpty().WithMessage("ارز الزامی است")
            .Must(c => c == "IRR" || c == "USD" || c == "EUR")
            .WithMessage("ارز باید یکی از موارد IRR، USD یا EUR باشد");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("توضیحات نمی‌تواند بیشتر از ۵۰۰ کاراکتر باشد");
    }
}
