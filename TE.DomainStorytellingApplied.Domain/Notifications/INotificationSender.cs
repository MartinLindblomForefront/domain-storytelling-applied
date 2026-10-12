namespace TE.DomainStorytellingApplied.Domain;

public interface INotificationSender
{
    void SendConfirmation(Id bookerId, Id reservationId);

    void SendRejection(Id bookerId, Id reservationId);
}
