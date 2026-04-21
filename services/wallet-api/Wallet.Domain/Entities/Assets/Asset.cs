namespace Wallet.Domain.Entities.Assets;

using Wallet.Domain.Entities;
using Wallet.Domain.ValueObjects;

public sealed class Asset
{
    // Construtor para o EF Core (materialização)
    private Asset() { }

    public Asset(Guid id, string ticker, string name, AssetType type)
    {
        if (id == Guid.Empty) throw new ArgumentException("Id is required.", nameof(id));
        if (string.IsNullOrWhiteSpace(ticker)) throw new ArgumentException("Ticker is required.", nameof(ticker));
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required.", nameof(name));

        Id = id;
        Ticker = ticker.Trim().ToUpperInvariant();
        Name = name.Trim();
        Type = type;
    }

    public Guid Id { get; private set; }
    public string Ticker { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public AssetType Type { get; private set; }
    public decimal Quantity { get; private set; }
    public Money AveragePrice { get; private set; } = Money.Zero();
    public decimal CurrentPrice { get; private set; }

    public ICollection<Transaction> Transactions { get; private set; } = new List<Transaction>();

    // Comportamento: Adicionar uma nova transação (Compra)
    public void AddPurchase(decimal quantity, Money price)
    {
        if (quantity <= 0) throw new ArgumentOutOfRangeException(nameof(quantity), "Quantidade deve ser positiva");
        ArgumentNullException.ThrowIfNull(price);

        if (price.IsNegative)
            throw new ArgumentOutOfRangeException(nameof(price), "Preço não pode ser negativo");

        // Lógica de Preço Médio (Simples)
        var totalCost = (Quantity * AveragePrice.Amount) + (quantity * price.Amount);
        Quantity += quantity;

        AveragePrice.ApplyAmountAndCurrency(totalCost / Quantity, price.Currency);

        Transactions.Add(new Transaction(
            Guid.NewGuid(),
            Id,
            quantity,
            price,
            DateTime.UtcNow));
    }

    public void UpdateCurrentPrice(decimal newPrice)
    {
        if (newPrice < 0) return;
        CurrentPrice = newPrice;
    }
}
