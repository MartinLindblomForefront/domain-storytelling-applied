namespace TE.DomainStorytellingApplied.Domain;

public sealed class Timeslot
{
    public Id Id { get; }
    public Id PremisesId { get; }
    public DateTimeOffset Start { get; }
    public DateTimeOffset End { get; }
    public bool IsReserved { get; private set; }

    public Timeslot(Id id, Id premisesId, DateTimeOffset start, DateTimeOffset end)
    {
        if (start >= end)
        {
            throw new ArgumentException("Start time must be before end time.", nameof(start));
        }

        Id = id;
        PremisesId = premisesId;
        Start = start;
        End = end;
    }

    public bool Overlaps(Timeslot other) => Start < other.End && other.Start < End;

    public bool IsInFutureOf(DateTimeOffset now) => Start > now;

    public void Reserve()
    {
        if (IsReserved)
        {
            throw new InvalidOperationException("Timeslot is already reserved.");
        }

        IsReserved = true;
    }

    public void Release() => IsReserved = false;
}
