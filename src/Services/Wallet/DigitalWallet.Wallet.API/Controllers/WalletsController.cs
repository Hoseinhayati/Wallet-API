using DigitalWallet.Wallet.Application.Wallets.Commands.CreateWallet;
using DigitalWallet.Wallet.Application.Wallets.Commands.DepositMoney;
using DigitalWallet.Wallet.Application.Wallets.Commands.WithdrawMoney;
using DigitalWallet.Wallet.Application.Wallets.Queries.GetWalletBalance;
using DigitalWallet.Wallet.Application.Wallets.Queries.GetWalletDetails;
using DigitalWallet.Wallet.Application.Wallets.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DigitalWallet.Wallet.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class WalletsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<WalletsController> _logger;

    public WalletsController(
        IMediator mediator,
        ILogger<WalletsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Create a new wallet
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(CreateWalletResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CreateWalletResponse>> CreateWallet(
        [FromBody] CreateWalletCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetWallet), new { id = result.WalletId }, result);
    }

    /// <summary>
    /// Deposit money into wallet
    /// </summary>
    [HttpPost("{id}/deposit")]
    [ProducesResponseType(typeof(DepositMoneyResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DepositMoneyResponse>> DepositMoney(
        Guid id,
        [FromBody] DepositMoneyRequest request)
    {
        var command = new DepositMoneyCommand
        {
            WalletId = id,
            Amount = request.Amount,
            Currency = request.Currency ?? "IRR",
            Description = request.Description ?? string.Empty
        };

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Withdraw money from wallet
    /// </summary>
    [HttpPost("{id}/withdraw")]
    [ProducesResponseType(typeof(WithdrawMoneyResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WithdrawMoneyResponse>> WithdrawMoney(
        Guid id,
        [FromBody] WithdrawMoneyRequest request)
    {
        var command = new WithdrawMoneyCommand
        {
            WalletId = id,
            Amount = request.Amount,
            Currency = request.Currency ?? "IRR",
            Description = request.Description ?? string.Empty
        };

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Get wallet balance information
    /// </summary>
    [HttpGet("{id}/balance")]
    [ProducesResponseType(typeof(WalletBalanceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WalletBalanceDto>> GetWalletBalance(Guid id)
    {
        var query = new GetWalletBalanceQuery { WalletId = id };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get wallet details by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(WalletDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WalletDto>> GetWallet(Guid id)
    {
        var query = new GetWalletDetailsQuery { WalletId = id };
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}

// Request models for Swagger
public record DepositMoneyRequest
{
    public decimal Amount { get; init; }
    public string? Currency { get; init; }
    public string? Description { get; init; }
}

public record WithdrawMoneyRequest
{
    public decimal Amount { get; init; }
    public string? Currency { get; init; }
    public string? Description { get; init; }
}
