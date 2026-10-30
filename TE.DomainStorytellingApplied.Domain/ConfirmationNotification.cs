namespace TE.DomainStorytellingApplied.Domain;


internal sealed class ConfirmationNotification(Booking booking)
{
    private readonly object gate = new();
    private volatile ConfirmationDeliveryResult result = new(ConfirmationDeliveryStatus.Pending);

    public ConfirmationDeliveryResult Result => result;

    public ConfirmationDeliveryResult TrySend(NotificationService notifications)
    {

        lock (gate)
        {
            if (result.Status is ConfirmationDeliveryStatus.Sent or ConfirmationDeliveryStatus.Suppressed)
            {
                return result;
            }

            if (!booking.IsConfirmed)
            {
                return result = new(ConfirmationDeliveryStatus.Suppressed);
            }

            var recipient = booking.Booker.Email;
            var message = $"Din bokning av {booking.Room.Name} är bekräftad: " +
                $"{booking.Period.Start:yyyy-MM-dd HH:mm}–{booking.Period.End:yyyy-MM-dd HH:mm}.";
            result = new(ConfirmationDeliveryStatus.Pending);
            try
            {
                notifications.Send(recipient, message);
                return result = new(ConfirmationDeliveryStatus.Sent);
            }
            catch (Exception error)
            {
                // Isolate delivery failures from the already successful payment.
                // Retain diagnostics and allow an explicit retry of this message only.
                return result = new(ConfirmationDeliveryStatus.Failed, error.Message);
            }
        }
    }
}
