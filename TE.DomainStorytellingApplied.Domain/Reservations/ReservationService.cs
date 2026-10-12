namespace TE.DomainStorytellingApplied.Domain;

public sealed class ReservationService(
    BookingPolicy policy,
    ITimeslotRepository timeslots,
    IReservationRepository reservations)
{
    public Reservation Reserve(Booker booker, Id timeslotId)
    {
        policy.Enforce(booker, timeslotId);

        var timeslot = timeslots.Find(timeslotId)!;
        timeslot.Reserve();

        var reservation = new Reservation(Id.New(), booker.Id, timeslot.PremisesId, timeslot.Id);
        reservations.Add(reservation);

        return reservation;
    }
}
