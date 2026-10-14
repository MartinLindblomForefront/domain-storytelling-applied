namespace TE.DomainStorytellingApplied.Domain;

public sealed class Notification
{
    private Notification(Guid recipientId, Guid reservationId, NotificationType type, string message)
    {
        RecipientId = recipientId;
        ReservationId = reservationId;
        Type = type;
        Message = message;
    }

    public Guid RecipientId { get; }

    public Guid ReservationId { get; }

    public NotificationType Type { get; }

    public string Message { get; }

    public static Notification For(Reservation reservation)
    {
        ArgumentNullException.ThrowIfNull(reservation);

        return reservation.Status switch
        {
            ReservationStatus.Approved => new Notification(
                reservation.BookerId,
                reservation.Id,
                NotificationType.Confirmation,
                "Your reservation has been confirmed."),
            ReservationStatus.Rejected => new Notification(
                reservation.BookerId,
                reservation.Id,
                NotificationType.Rejection,
                $"Your reservation has been rejected. Reason: {reservation.RejectionReason}"),
            _ => throw new InvalidOperationException(
                $"Cannot notify about a reservation with status {reservation.Status}.")
        };
    }
}
