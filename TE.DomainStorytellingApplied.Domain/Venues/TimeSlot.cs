namespace TE.DomainStorytellingApplied.Domain;

public readonly record struct TimeSlot
{
    public DateTime Start { get; }

    public TimeSlot(DateTime start)
    {
        if (start.Minute != 0 || start.Second != 0 || start.Millisecond != 0)
        {
            throw new DomainException("En tidsslot måste starta på en hel timme.");
        }

        Start = start;
    }
}
