using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DigitalWallet.Transaction.Infrastructure.Persistence;

/// <summary>
/// Initializer for database migration and seeding
/// </summary>
public class ApplicationDbContextInitializer
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ApplicationDbContextInitializer> _logger;

    public ApplicationDbContextInitializer(
        ApplicationDbContext context,
        ILogger<ApplicationDbContextInitializer> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Initialize database (migrate)
    /// </summary>
    public async Task InitializeAsync()
    {
        try
        {
            // Apply migrations
            if (_context.Database.IsSqlServer())
            {
                await _context.Database.MigrateAsync();
                _logger.LogInformation("Transaction database migrated successfully");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while migrating the transaction database");
            throw;
        }
    }

    /// <summary>
    /// Seed initial data (if needed)
    /// </summary>
    public async Task SeedAsync()
    {
        try
        {
            await TrySeedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the transaction database");
            throw;
        }
    }

    private async Task TrySeedAsync()
    {
        // Check if database has data
        if (!await _context.Transactions.AnyAsync())
        {
            _logger.LogInformation("Transaction database is empty. No seed data needed.");
        }
        else
        {
            _logger.LogInformation("Transaction database already contains data. Skipping seed.");
        }
    }
}
