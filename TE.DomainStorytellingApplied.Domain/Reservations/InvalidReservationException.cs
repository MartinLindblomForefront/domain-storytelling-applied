namespace TE.DomainStorytellingApplied.Domain.Reservations;

public sealed class InvalidReservationException : DomainException
{
    private InvalidReservationException(string message) : base(message) { }

    public static InvalidReservationException EmptyId() => new("Reservation id must not be empty.");
}
