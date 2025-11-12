using DigitalWallet.Wallet.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalWallet.Wallet.Infrastructure.Persistence.Configurations;

public class WalletConfiguration : IEntityTypeConfiguration<Domain.Entities.Wallet>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.Wallet> builder)
    {
        builder.ToTable("Wallets");

        builder.HasKey(w => w.Id);

        builder.Property(w => w.UserId)
            .IsRequired()
            .HasMaxLength(11);

        builder.Property(w => w.DisplayName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(w => w.Currency)
            .IsRequired()
            .HasMaxLength(3);

        builder.Property(w => w.Status)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(w => w.Version)
            .IsRequired()
            .IsConcurrencyToken();

        // Configure WalletCode value object
        builder.OwnsOne(w => w.Code, code =>
        {
            code.Property(c => c.Value)
                .HasColumnName("Code")
                .IsRequired()
                .HasMaxLength(20);

            code.HasIndex(c => c.Value)
                .IsUnique();
        });

        // Configure Money value object
        builder.OwnsOne(w => w.Balance, balance =>
        {
            balance.Property(m => m.Amount)
                .HasColumnName("Balance")
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            balance.Property(m => m.Currency)
                .HasColumnName("BalanceCurrency")
                .IsRequired()
                .HasMaxLength(3);
        });

        // Ignore domain events
        builder.Ignore(w => w.DomainEvents);

        // Index for UserId
        builder.HasIndex(w => w.UserId);
    }
}
