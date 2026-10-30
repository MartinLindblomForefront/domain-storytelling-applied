namespace TE.DomainStorytellingApplied.Domain;

public readonly record struct Money
{
    public decimal Amount { get; }

    public Money(decimal amount)
    {
        if (amount < 0)
        {
            throw new DomainException("Ett belopp kan inte vara negativt.");
        }

        Amount = amount;
    }

    public static Money Zero
    {
        get { return new Money(0); }
    }

    public bool IsZero
    {
        get { return Amount == 0; }
    }
}
