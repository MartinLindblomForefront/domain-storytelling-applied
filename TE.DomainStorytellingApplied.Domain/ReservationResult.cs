namespace TE.DomainStorytellingApplied.Domain;

public enum ReservationOutcome
{
    Reserved,
    RoomUnavailable
}

public sealed class ReservationResult
{
    internal ReservationResult(Booking? booking)
    {
        Booking = booking;
    }

    public Booking? Booking { get; }
    public ReservationOutcome Outcome => Booking is null
        ? ReservationOutcome.RoomUnavailable
        : ReservationOutcome.Reserved;
}
