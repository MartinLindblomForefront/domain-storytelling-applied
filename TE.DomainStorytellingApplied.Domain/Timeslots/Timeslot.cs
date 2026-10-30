namespace TE.DomainStorytellingApplied.Domain.Timeslots;

// Domain value object
public sealed record Timeslot
{
    public DateTimeOffset Start { get; }
    public DateTimeOffset End { get; }
    public TimeSpan Duration => End - Start;

    public Timeslot(DateTimeOffset start, DateTimeOffset end)
    {
        if (end <= start)
        {
            throw InvalidTimeslotException.EndMustBeAfterStart(start, end);
        }

        Start = start;
        End = end;
    }
}
