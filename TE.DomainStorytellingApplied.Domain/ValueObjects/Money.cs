namespace TE.DomainStorytellingApplied.Domain;

/// <summary>
/// A value object: an amount of money in a specific currency.
/// It is immutable, and because it is a record, two Money objects with the same amount and currency are equal.
/// Why not a plain decimal: one place for the rule, a type the compiler can tell apart from other numbers,
/// and a place for rounding when the "50 minutes costs 116,666... kr" problem turns up.
/// </summary>
public sealed record Money
{
    // No setters: the values are set once in the constructor, so the rules checked there can never be broken later.
    public decimal Amount { get; }

    public string Currency { get; }

    public Money(decimal amount, string currency)
    {
        if (amount < 0)
        {
            throw new ArgumentException("Amount cannot be negative.", nameof(amount));
        }

        if (string.IsNullOrWhiteSpace(currency))
        {
            throw new ArgumentException("Currency is required.", nameof(currency));
        }

        Amount = amount;
        Currency = currency;
    }

    // Returns a new Money and leaves this one unchanged. The result is rounded to whole öre (two decimals).
    public Money Multiply(decimal factor)
    {
        if (factor < 0)
        {
            throw new ArgumentException("Cannot multiply money by a negative number.", nameof(factor));
        }

        var rounded = Math.Round(Amount * factor, 2, MidpointRounding.AwayFromZero);

        return new Money(rounded, Currency);
    }
}
