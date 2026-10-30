namespace TE.DomainStorytellingApplied.Domain;

/// <summary>
/// A value object: a period of time with a start and an end.
/// It is immutable, and because it is a record, two time slots with the same start and end are equal.
/// </summary>
public sealed record TimeSlot
{
    // No setters: the values are set once in the constructor, so the rule checked there can never be broken later.
    public DateTime Start { get; }

    public DateTime End { get; }

    public TimeSlot(DateTime start, DateTime end)
    {
        if (end <= start)
        {
            throw new ArgumentException("End time must be after start time.", nameof(end));
        }

        Start = start;
        End = end;
    }

    public TimeSpan Duration => End - Start;

    // Two slots that only touch (one ends exactly when the other starts) do not overlap.
    public bool OverlapsWith(TimeSlot other)
    {
        return Start < other.End && other.Start < End;
    }
}
