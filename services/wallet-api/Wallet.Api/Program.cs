using System.Text.Json;
using MassTransit;
using Wallet.Infrastructure;
using Wallet.Application;
using Wallet.Application.Consumers;
using Wallet.Application.DTOs;
using Microsoft.EntityFrameworkCore;
using Wallet.Application.UseCases.BuyAsset;
using Wallet.Application.UseCases.GetAssets;
using Wallet.Infrastructure.Persistence;
using Serilog;
using Wallet.Api.Middleware;

var builder = WebApplication.CreateBuilder(args);

// 1. CHAMA A EXTENSÃO DA INFRASTRUCTURE
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

// No topo do Program.cs
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .Enrich.WithThreadId()
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.SerializerOptions.PropertyNameCaseInsensitive = true;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

builder.Services.AddMassTransit(x =>
{
    // Registra o Consumer
    x.AddConsumer<AssetPriceUpdatedConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        // Configura o endpoint da fila para este consumidor específico
        cfg.ReceiveEndpoint("wallet-price-update-queue", e =>
        {
            e.ConfigureConsumer<AssetPriceUpdatedConsumer>(context);
        });
    });
});

var app = builder.Build();

// Ativa o tratamento de erros global
app.UseMiddleware<Wallet.Api.Middleware.ExceptionHandlingMiddleware>();
app.UseMiddleware<TracingMiddleware>();
app.UseMiddleware<GatewayUserMiddleware>();

// Executa as migrações automaticamente ao iniciar
await Wallet.Infrastructure.Persistence.DbInitializer.ApplyMigrations(app.Services);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

// ENDPOINT: Listar Ativos
app.MapGet("/v1/assets", async (GetAssetsQuery query) =>
{
    var result = await query.ExecuteAsync();
    return Results.Ok(result);
})
.WithName("GetAssets");

// ENDPOINT: Comprar Ativo
app.MapPost("/v1/assets/buy", async (BuyAssetUseCase useCase, BuyAssetRequest request) =>
{
    await useCase.ExecuteAsync(request);
    return Results.Ok();
})
.WithName("BuyAsset");

// ENDPOINT: Histórico de Transações por Ticker
app.MapGet("/v1/assets/{id}/transactions", async (Guid id, WalletDbContext db) =>
{
    var transactions = await db.Transactions
        .Where(t => t.AssetId == id)
        .OrderByDescending(t => t.CreatedAt)
        .ToListAsync();

    return Results.Ok(transactions);
})
.WithName("GetAssetTransactions");

app.Run();

