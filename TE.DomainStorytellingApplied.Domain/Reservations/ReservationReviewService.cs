namespace TE.DomainStorytellingApplied.Domain;

public sealed class ReservationReviewService(ReservationNotifier notifier, ITimeslotRepository timeslots)
{
    public void Approve(Reservation reservation, Approver approver)
    {
        reservation.Approve(approver.Id);
        notifier.Notify(reservation);
    }

    public void Reject(Reservation reservation, Approver approver)
    {
        reservation.Reject(approver.Id);
        timeslots.Find(reservation.TimeslotId)?.Release();
        notifier.Notify(reservation);
    }
}
