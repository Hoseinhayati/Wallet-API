using Microsoft.EntityFrameworkCore;

namespace DigitalWallet.Transaction.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Domain.Entities.Transaction> Transactions { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
