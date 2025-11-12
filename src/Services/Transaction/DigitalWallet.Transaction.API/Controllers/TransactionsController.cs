using DigitalWallet.Transaction.Application.Transactions.Commands.RecordTransaction;
using DigitalWallet.Transaction.Application.Transactions.Queries.GetTransaction;
using DigitalWallet.Transaction.Application.Transactions.Queries.GetTransactionsByUser;
using DigitalWallet.Transaction.Application.Transactions.Queries.GetTransactionsByWallet;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DigitalWallet.Transaction.API.Controllers;

/// <summary>
/// Controller for managing transactions
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class TransactionsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<TransactionsController> _logger;

    public TransactionsController(
        IMediator mediator,
        ILogger<TransactionsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Record a new transaction
    /// </summary>
    /// <param name="command">Transaction details</param>
    /// <returns>Transaction information with reference code</returns>
    [HttpPost]
    [ProducesResponseType(typeof(RecordTransactionResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RecordTransactionResponse>> RecordTransaction(
        [FromBody] RecordTransactionCommand command)
    {
        _logger.LogInformation(
            "Recording transaction: Type={Type}, WalletId={WalletId}, Amount={Amount}",
            command.Type,
            command.WalletId,
            command.Amount);

        var result = await _mediator.Send(command);

        return CreatedAtAction(
            nameof(GetTransaction),
            new { id = result.TransactionId },
            result);
    }

    /// <summary>
    /// Get transaction by ID
    /// </summary>
    /// <param name="id">Transaction ID</param>
    /// <returns>Transaction details</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTransaction(Guid id)
    {
        var query = new GetTransactionQuery { TransactionId = id };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get all transactions for a specific wallet
    /// </summary>
    /// <param name="walletId">Wallet ID</param>
    /// <param name="type">Filter by transaction type (optional)</param>
    /// <param name="status">Filter by status (optional)</param>
    /// <param name="startDate">Start date filter (optional)</param>
    /// <param name="endDate">End date filter (optional)</param>
    /// <param name="take">Number of records to return (default: 100)</param>
    /// <param name="skip">Number of records to skip (default: 0)</param>
    /// <returns>List of transactions</returns>
    [HttpGet("wallet/{walletId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTransactionsByWallet(
        Guid walletId,
        [FromQuery] string? type = null,
        [FromQuery] string? status = null,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] int take = 100,
        [FromQuery] int skip = 0)
    {
        var query = new GetTransactionsByWalletQuery
        {
            WalletId = walletId,
            Type = type,
            Status = status,
            StartDate = startDate,
            EndDate = endDate,
            Take = take,
            Skip = skip
        };

        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get all transactions for a specific user
    /// </summary>
    /// <param name="userId">User ID (National Code)</param>
    /// <param name="type">Filter by transaction type (optional)</param>
    /// <param name="status">Filter by status (optional)</param>
    /// <param name="startDate">Start date filter (optional)</param>
    /// <param name="endDate">End date filter (optional)</param>
    /// <param name="take">Number of records to return (default: 100)</param>
    /// <param name="skip">Number of records to skip (default: 0)</param>
    /// <returns>List of transactions</returns>
    [HttpGet("user/{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTransactionsByUser(
        string userId,
        [FromQuery] string? type = null,
        [FromQuery] string? status = null,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] int take = 100,
        [FromQuery] int skip = 0)
    {
        var query = new GetTransactionsByUserQuery
        {
            UserId = userId,
            Type = type,
            Status = status,
            StartDate = startDate,
            EndDate = endDate,
            Take = take,
            Skip = skip
        };

        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get transaction statistics for a wallet
    /// </summary>
    /// <param name="walletId">Wallet ID</param>
    /// <returns>Transaction statistics</returns>
    [HttpGet("wallet/{walletId}/stats")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetWalletStatistics(Guid walletId)
    {
        // Get all transactions for the wallet
        var query = new GetTransactionsByWalletQuery
        {
            WalletId = walletId,
            Take = 1000 // Get more for statistics
        };

        var transactions = await _mediator.Send(query);

        // Calculate statistics
        var stats = new
        {
            TotalTransactions = transactions.Count,
            TotalDeposits = transactions.Count(t => t.Type == "Deposit"),
            TotalWithdrawals = transactions.Count(t => t.Type == "Withdrawal"),
            TotalDepositAmount = transactions
                .Where(t => t.Type == "Deposit")
                .Sum(t => t.Amount),
            TotalWithdrawalAmount = transactions
                .Where(t => t.Type == "Withdrawal")
                .Sum(t => t.Amount),
            LastTransaction = transactions
                .OrderByDescending(t => t.TransactionDate)
                .FirstOrDefault()
        };

        return Ok(stats);
    }
}
