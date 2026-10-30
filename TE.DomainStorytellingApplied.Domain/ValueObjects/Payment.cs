namespace TE.DomainStorytellingApplied.Domain;

/// <summary>
/// A value object: one payment, with how much was paid, how, and when.
/// It is immutable, and because it is a record, two Payments with the same values are equal.
/// It has no rule of its own: Money already guards the amount, and the enum guards the method.
/// </summary>
public sealed record Payment
{
    public Money Amount { get; init; }

    public PaymentMethod Method { get; init; }

    public DateTime PaidAt { get; init; }

    public Payment(Money amount, PaymentMethod method, DateTime paidAt)
    {
        Amount = amount;
        Method = method;
        PaidAt = paidAt;
    }
}
