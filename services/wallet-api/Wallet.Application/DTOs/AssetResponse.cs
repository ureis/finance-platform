namespace Wallet.Application.DTOs;

public record AssetResponse(
    Guid Id,
    string Ticker,
    string Name,
    string Type,
    decimal Quantity,
    decimal AveragePrice,
    string Currency,
    decimal CurrentPrice
);