namespace TE.DomainStorytellingApplied.Domain;

/// <summary>
/// A value object: the time of day a venue opens and closes.
/// It is immutable, and because it is a record, two OpeningHours with the same times are equal.
/// </summary>
public sealed record OpeningHours
{
    // No setters: the values are set once in the constructor, so the rule checked there can never be broken later.
    public TimeOnly Opens { get; }

    public TimeOnly Closes { get; }

    public OpeningHours(TimeOnly opens, TimeOnly closes)
    {
        if (closes <= opens)
        {
            throw new ArgumentException("Closing time must be after opening time.", nameof(closes));
        }

        Opens = opens;
        Closes = closes;
    }

    // A slot is covered when it starts and ends on the same day, and lies within the opening hours.
    public bool Covers(TimeSlot timeSlot)
    {
        return timeSlot.Start.Date == timeSlot.End.Date
            && TimeOnly.FromDateTime(timeSlot.Start) >= Opens
            && TimeOnly.FromDateTime(timeSlot.End) <= Closes;
    }
}
