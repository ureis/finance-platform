namespace Quotes.Contracts;

public record AssetPriceUpdatedEvent(
    string Ticker,
    decimal NewPrice,
    DateTime UpdatedAt
);