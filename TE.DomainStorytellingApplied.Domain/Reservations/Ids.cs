namespace TE.DomainStorytellingApplied.Domain.Reservations;

public readonly record struct ReservationId(Guid Value)
{
    public static ReservationId New() => new(Guid.NewGuid());
}

public readonly record struct BookerId(Guid Value);
