namespace DigitalWallet.Transaction.Application.Transactions.DTOs;

public record TransactionDto
{
    public Guid Id { get; init; }
    public Guid WalletId { get; init; }
    public string UserId { get; init; } = string.Empty;
    public string Type { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public string Currency { get; init; } = "IRR";
    public string Status { get; init; } = string.Empty;
    public string ReferenceCode { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public decimal BalanceAfterTransaction { get; init; }
    public DateTime TransactionDate { get; init; }
    public DateTime CreatedDate { get; init; }
}
