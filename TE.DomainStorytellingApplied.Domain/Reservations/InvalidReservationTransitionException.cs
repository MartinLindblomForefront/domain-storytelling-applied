namespace TE.DomainStorytellingApplied.Domain.Reservations;

public sealed class InvalidReservationTransitionException : DomainException
{
    private InvalidReservationTransitionException(string message) : base(message) { }

    public static InvalidReservationTransitionException Cannot(string action, ReservationStatus status) =>
        new($"Cannot {action} a reservation with status {status}.");
}
