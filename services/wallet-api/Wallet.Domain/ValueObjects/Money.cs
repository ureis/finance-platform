namespace Wallet.Domain.ValueObjects;

public sealed class Money
{
    // Construtor para o EF Core (materialização)
    private Money() { }

    public decimal Amount { get; private set; }
    public string Currency { get; private set; } = "BRL";

    private Money(decimal amount, string currency)
    {
        if (string.IsNullOrWhiteSpace(currency))
            throw new ArgumentException("Currency is required.", nameof(currency));

        Currency = currency.Trim().ToUpperInvariant();
        Amount = Normalize(amount, Currency);
    }

    public static Money Of(decimal amount, string currency = "BRL") => new(amount, currency);

    public static Money Zero(string currency = "BRL") => new(0m, currency);

    /// <summary>
    /// Atualiza valor e moeda in-place para o EF Core rastrear Owned Types sem trocar a referência.
    /// </summary>
    public void ApplyAmountAndCurrency(decimal amount, string currency)
    {
        if (string.IsNullOrWhiteSpace(currency))
            throw new ArgumentException("Currency is required.", nameof(currency));

        Currency = currency.Trim().ToUpperInvariant();
        Amount = Normalize(amount, Currency);
    }

    public bool IsZero => Amount == 0m;

    public bool IsNegative => Amount < 0m;

    public Money Abs() => new(Math.Abs(Amount), Currency);

    public override string ToString() => $"{Currency} {Amount:N2}";

    public static Money operator +(Money left, Money right)
    {
        EnsureSameCurrency(left, right);
        return new Money(left.Amount + right.Amount, left.Currency);
    }

    public static Money operator -(Money left, Money right)
    {
        EnsureSameCurrency(left, right);
        return new Money(left.Amount - right.Amount, left.Currency);
    }

    public static Money operator *(Money money, decimal multiplier) => new(money.Amount * multiplier, money.Currency);

    public static Money operator /(Money money, decimal divisor)
    {
        if (divisor == 0m) throw new DivideByZeroException();
        return new Money(money.Amount / divisor, money.Currency);
    }

    public static bool operator >(Money left, Money right)
    {
        EnsureSameCurrency(left, right);
        return left.Amount > right.Amount;
    }

    public static bool operator <(Money left, Money right)
    {
        EnsureSameCurrency(left, right);
        return left.Amount < right.Amount;
    }

    public static bool operator >=(Money left, Money right)
    {
        EnsureSameCurrency(left, right);
        return left.Amount >= right.Amount;
    }

    public static bool operator <=(Money left, Money right)
    {
        EnsureSameCurrency(left, right);
        return left.Amount <= right.Amount;
    }

    private static void EnsureSameCurrency(Money left, Money right)
    {
        if (!string.Equals(left.Currency, right.Currency, StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException($"Currency mismatch: '{left.Currency}' vs '{right.Currency}'.");
    }

    private static decimal Normalize(decimal amount, string currency)
    {
        var scale = GetMinorUnitScale(currency);
        return decimal.Round(amount, scale, MidpointRounding.ToEven);
    }

    private static int GetMinorUnitScale(string currency) =>
        currency.ToUpperInvariant() switch
        {
            "JPY" => 0,
            "KWD" => 3,
            "BHD" => 3,
            _ => 2
        };
}
