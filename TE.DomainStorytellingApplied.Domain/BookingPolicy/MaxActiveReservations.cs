namespace TE.DomainStorytellingApplied.Domain;

public sealed record MaxActiveReservations
{
    public int Value { get; }

    public MaxActiveReservations(int value)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value);
        Value = value;
    }
}
