namespace TE.DomainStorytellingApplied.Domain;

public sealed record Timeslot
{
    public Timeslot(DateTimeOffset start, DateTimeOffset end)
    {
        if (start >= end)
        {
            throw new ArgumentException("Start must be before end.", nameof(start));
        }

        Start = start;
        End = end;
    }

    public DateTimeOffset Start { get; }

    public DateTimeOffset End { get; }

    public TimeSpan Duration => End - Start;

    public bool Overlaps(Timeslot other)
    {
        ArgumentNullException.ThrowIfNull(other);

        return Start < other.End && other.Start < End;
    }
}
