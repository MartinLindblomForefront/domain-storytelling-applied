namespace TE.DomainStorytellingApplied.Domain.Aggregates.Bookers.ValueObjects;

public sealed record BookerId
{
    public BookerId(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException("Booker ID cannot be empty.", nameof(value));
        }

        Value = value;
    }

    public Guid Value { get; }
}
