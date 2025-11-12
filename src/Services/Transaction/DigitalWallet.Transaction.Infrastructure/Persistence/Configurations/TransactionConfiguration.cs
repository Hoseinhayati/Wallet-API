using DigitalWallet.Transaction.Domain.Entities;
using DigitalWallet.Transaction.Domain.Enums;
using DigitalWallet.Transaction.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalWallet.Transaction.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for Transaction entity
/// </summary>
public class TransactionConfiguration : IEntityTypeConfiguration<Domain.Entities.Transaction>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.Transaction> builder)
    {
        // Table name
        builder.ToTable("Transactions");

        // Primary key
        builder.HasKey(t => t.Id);

        // Properties
        builder.Property(t => t.WalletId)
            .IsRequired();

        builder.Property(t => t.UserId)
            .IsRequired()
            .HasMaxLength(11);

        builder.Property(t => t.Type)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(t => t.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(t => t.Description)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(t => t.TransactionDate)
            .IsRequired();

        builder.Property(t => t.BalanceAfterTransaction)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(t => t.Metadata)
            .HasMaxLength(4000);

        builder.Property(t => t.CreatedDate)
            .IsRequired();

        builder.Property(t => t.UpdatedDate);

        builder.Property(t => t.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(t => t.IsActivated)
            .IsRequired()
            .HasDefaultValue(true);

        // Value Objects - ReferenceCode
        builder.OwnsOne(t => t.ReferenceCode, refBuilder =>
        {
            refBuilder.Property(r => r.Value)
                .HasColumnName("ReferenceCode")
                .IsRequired()
                .HasMaxLength(50);

            // Unique constraint
            refBuilder.HasIndex(r => r.Value)
                .IsUnique();
        });

        // Value Objects - Money (Amount)
        builder.OwnsOne(t => t.Amount, moneyBuilder =>
        {
            moneyBuilder.Property(m => m.Amount)
                .HasColumnName("Amount")
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            moneyBuilder.Property(m => m.Currency)
                .HasColumnName("Currency")
                .IsRequired()
                .HasMaxLength(3)
                .HasDefaultValue("IRR");
        });

        // Indexes
        builder.HasIndex(t => t.WalletId);
        builder.HasIndex(t => t.UserId);
        builder.HasIndex(t => t.Type);
        builder.HasIndex(t => t.Status);
        builder.HasIndex(t => t.TransactionDate);

        // Composite index for common queries
        builder.HasIndex(t => new { t.WalletId, t.TransactionDate });
        builder.HasIndex(t => new { t.UserId, t.TransactionDate });

        // Query filters
        builder.HasQueryFilter(t => !t.IsDeleted);

        // Ignore Domain Events (navigation property)
        builder.Ignore(t => t.DomainEvents);
    }
}
