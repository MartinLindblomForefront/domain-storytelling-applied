namespace TE.DomainStorytellingApplied.Domain;

public record TimePeriod
{
    public bool Overlaps(TimePeriod other)
    {
        ArgumentNullException.ThrowIfNull(other);
        return Start < other.End && End > other.Start;
    }

    public DateTime Start { get; }
    public DateTime End { get; }
    public TimePeriod(DateTime start, DateTime end)
    {
        if (end <= start)
        {
            throw new ArgumentException("End must be later than start.", nameof(end));
        }

        Start = start;
        End = end;
    }
}
