namespace TE.DomainStorytellingApplied.Domain;

public sealed class ReservationNotifier(INotificationSender sender)
{
    public void Notify(Reservation reservation)
    {
        switch (reservation.Status)
        {
            case ReservationStatus.Approved:
                sender.SendConfirmation(reservation.BookerId, reservation.Id);
                break;
            case ReservationStatus.Rejected:
                sender.SendRejection(reservation.BookerId, reservation.Id);
                break;
        }
    }
}
