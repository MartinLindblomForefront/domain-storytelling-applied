namespace TE.DomainStorytellingApplied.Domain;

public sealed record PolicyViolation(string Code, string Message)
{
    public static readonly PolicyViolation TimeslotNotFound =
        new(nameof(TimeslotNotFound), "The timeslot does not exist for these premises.");

    public static readonly PolicyViolation TimeslotInPast =
        new(nameof(TimeslotInPast), "The timeslot must start in the future.");

    public static readonly PolicyViolation TimeslotAlreadyReserved =
        new(nameof(TimeslotAlreadyReserved), "The timeslot is already reserved.");

    public static readonly PolicyViolation MaxActiveReservationsReached =
        new(nameof(MaxActiveReservationsReached), "The booker has reached the maximum number of active reservations.");
}
