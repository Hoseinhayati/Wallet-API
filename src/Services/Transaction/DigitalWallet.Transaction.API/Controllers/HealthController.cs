using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DigitalWallet.Transaction.Infrastructure.Persistence;

namespace DigitalWallet.Transaction.API.Controllers;

/// <summary>
/// Health check controller
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<HealthController> _logger;

    public HealthController(
        ApplicationDbContext context,
        ILogger<HealthController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Basic health check
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Get()
    {
        return Ok(new
        {
            Service = "Transaction Service",
            Status = "Healthy",
            Timestamp = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Database health check
    /// </summary>
    [HttpGet("database")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> CheckDatabase()
    {
        try
        {
            // Try to connect to database
            await _context.Database.CanConnectAsync();

            var transactionCount = await _context.Transactions.CountAsync();

            return Ok(new
            {
                Status = "Healthy",
                Database = "Connected",
                TransactionCount = transactionCount,
                Timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database health check failed");
            return StatusCode(503, new
            {
                Status = "Unhealthy",
                Database = "Disconnected",
                Error = ex.Message,
                Timestamp = DateTime.UtcNow
            });
        }
    }
}
