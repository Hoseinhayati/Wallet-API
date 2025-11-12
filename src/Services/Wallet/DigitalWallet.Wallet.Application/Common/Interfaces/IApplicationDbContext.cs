using Microsoft.EntityFrameworkCore;

namespace DigitalWallet.Wallet.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Domain.Entities.Wallet> Wallets { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
