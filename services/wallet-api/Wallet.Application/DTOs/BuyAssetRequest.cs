namespace Wallet.Application.DTOs;

public record BuyAssetRequest(
    string Ticker,
    string Name,
    string Type, // CDB, Stocks, etc
    decimal Quantity,
    decimal Price
);