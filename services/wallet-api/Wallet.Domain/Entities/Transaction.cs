using Wallet.Domain.ValueObjects;

namespace Wallet.Domain.Entities;

public sealed class Transaction
{
    private Transaction() { }

    public Transaction(Guid id, Guid assetId, decimal quantity, Money priceAtTime, DateTime createdAt)
    {
        Id = id;
        AssetId = assetId;
        Quantity = quantity;
        PriceAtTime = priceAtTime;
        CreatedAt = createdAt;
    }

    public Guid Id { get; private set; }
    public Guid AssetId { get; private set; }
    public decimal Quantity { get; private set; }
    public Money PriceAtTime { get; private set; } = null!;
    public DateTime CreatedAt { get; private set; }
}
