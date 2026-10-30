namespace TE.DomainStorytellingApplied.Domain.Timeslots;

public readonly record struct TimeslotId(Guid Value)
{
    public static TimeslotId New() => new(Guid.NewGuid());
}

public readonly record struct PremisesId(Guid Value);
