namespace TE.DomainStorytellingApplied.Domain.Reservations;

public sealed record TimeSlot
{
    private TimeSlot(DateTimeOffset startsAt, DateTimeOffset endsAt)
    {
        StartsAt = startsAt;
        EndsAt = endsAt;
    }

    public DateTimeOffset StartsAt { get; }
    public DateTimeOffset EndsAt { get; }
    public TimeSpan Duration => EndsAt - StartsAt;

    public static TimeSlot Create(DateTimeOffset startsAt, DateTimeOffset endsAt)
    {
        if (endsAt <= startsAt)
        {
            throw new ArgumentException("Sluttiden måste vara efter starttiden.", nameof(endsAt));
        }

        return new TimeSlot(startsAt, endsAt);
    }

    public bool Overlaps(TimeSlot other)
    {
        ArgumentNullException.ThrowIfNull(other);

        return StartsAt < other.EndsAt && other.StartsAt < EndsAt;
    }
}
