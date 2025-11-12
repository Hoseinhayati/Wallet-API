using FluentValidation;

namespace DigitalWallet.Wallet.Application.Wallets.Commands.CreateWallet;

/// <summary>
/// Validator for CreateWalletCommand
/// </summary>
public class CreateWalletCommandValidator : AbstractValidator<CreateWalletCommand>
{
    public CreateWalletCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("شناسه کاربری الزامی است")
            .Length(10, 11).WithMessage("شناسه کاربری باید ۱۰ یا ۱۱ رقم باشد (کد ملی)")
            .Matches(@"^\d+$").WithMessage("شناسه کاربری فقط باید شامل اعداد باشد");

        RuleFor(x => x.DisplayName)
            .NotEmpty().WithMessage("نام نمایشی الزامی است")
            .MinimumLength(3).WithMessage("نام نمایشی باید حداقل ۳ کاراکتر باشد")
            .MaximumLength(100).WithMessage("نام نمایشی نمی‌تواند بیشتر از ۱۰۰ کاراکتر باشد");

        RuleFor(x => x.Currency)
            .NotEmpty().WithMessage("ارز الزامی است")
            .Must(c => c == "IRR" || c == "USD" || c == "EUR")
            .WithMessage("ارز باید یکی از موارد IRR، USD یا EUR باشد");
    }
}
