using DigitalWallet.Wallet.API.Middleware;
using DigitalWallet.Wallet.Application;
using DigitalWallet.Wallet.Infrastructure;
using DigitalWallet.Wallet.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Serilog;

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(new ConfigurationBuilder()
        .AddJsonFile("appsettings.json")
        .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", true)
        .Build())
    .Enrich.FromLogContext()
    .CreateLogger();

try
{
    Log.Information("🚀 Starting DigitalWallet.Wallet.API...");

var builder = WebApplication.CreateBuilder(args);

// Use Serilog for logging
builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddControllers();

// Add API Explorer and Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Digital Wallet API",
        Version = "v1",
        Description = "API for managing digital wallets, deposits, and withdrawals"
    });
});

// Add Application Layer services (MediatR, FluentValidation, etc.)
builder.Services.AddApplicationServices();

// Add Infrastructure Layer services (DbContext, Repositories, etc.)
builder.Services.AddInfrastructureServices(builder.Configuration);

// Add CORS (optional, for development)
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Global exception handler (must be first in the pipeline)
app.UseGlobalExceptionHandler();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Digital Wallet API v1");
        options.RoutePrefix = string.Empty; // Set Swagger UI at app's root
    });
}

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthorization();

app.MapControllers();

// Auto-migrate database on startup (Development only)
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await dbContext.Database.MigrateAsync();
}

    Log.Information("✅ DigitalWallet.Wallet.API started successfully");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "❌ Application failed to start");
    throw;
}
finally
{
    Log.CloseAndFlush();
}
