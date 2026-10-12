namespace TE.DomainStorytellingApplied.Domain.Tests.Fakes;

public sealed class FakeNotificationSender : INotificationSender
{
    public List<(Id BookerId, Id ReservationId)> Confirmations { get; } = new();
    public List<(Id BookerId, Id ReservationId)> Rejections { get; } = new();

    public void SendConfirmation(Id bookerId, Id reservationId) => Confirmations.Add((bookerId, reservationId));

    public void SendRejection(Id bookerId, Id reservationId) => Rejections.Add((bookerId, reservationId));
}
