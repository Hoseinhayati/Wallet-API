using DigitalWallet.Wallet.Application.Common.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DigitalWallet.Wallet.Application.Wallets.Commands.CreateWallet;

/// <summary>
/// Handler for CreateWalletCommand
/// </summary>
public class CreateWalletCommandHandler
    : IRequestHandler<CreateWalletCommand, CreateWalletResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<CreateWalletCommandHandler> _logger;

    public CreateWalletCommandHandler(
        IApplicationDbContext context,
        ILogger<CreateWalletCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<CreateWalletResponse> Handle(
        CreateWalletCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Creating wallet for user {UserId} with display name {DisplayName}",
            request.UserId,
            request.DisplayName);

        // Create wallet using domain factory method
        var wallet = Domain.Entities.Wallet.Create(
            request.UserId,
            request.DisplayName,
            request.Currency);

        // Add to context
        _context.Wallets.Add(wallet);

        // Save changes (Domain Events will be dispatched here)
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Wallet created successfully with ID {WalletId} and code {WalletCode}",
            wallet.Id,
            wallet.Code.Value);

        return new CreateWalletResponse
        {
            WalletId = wallet.Id,
            UserId = wallet.UserId,
            WalletCode = wallet.Code.Value,
            DisplayName = wallet.DisplayName,
            Balance = wallet.Balance.Amount,
            Currency = wallet.Currency,
            CreatedDate = wallet.CreatedDate,
            Message = "Wallet created successfully",
            Success = true
        };
    }
}
