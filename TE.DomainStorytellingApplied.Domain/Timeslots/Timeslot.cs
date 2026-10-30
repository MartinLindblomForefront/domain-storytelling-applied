using TE.DomainStorytellingApplied.Domain.Reservations;
using TE.DomainStorytellingApplied.Domain.Shared;
using TE.DomainStorytellingApplied.Domain.Timeslots.Rules;

namespace TE.DomainStorytellingApplied.Domain.Timeslots;

public sealed class Timeslot
{
    private Timeslot(TimeslotId id, PremisesId premisesId, Period period)
    {
        Id = id;
        PremisesId = premisesId;
        Period = period;
        Status = TimeslotStatus.Available;
    }

    public TimeslotId Id { get; }

    public PremisesId PremisesId { get; }

    public Period Period { get; }

    public TimeslotStatus Status { get; private set; }

    public ReservationId? ReservationId { get; private set; }

    public static Timeslot CreateAvailable(PremisesId premisesId, Period period) =>
        new(TimeslotId.New(), premisesId, period);

    public void Reserve(ReservationId reservationId)
    {
        Rule.Check(new TimeslotMustBeAvailable(this));

        Status = TimeslotStatus.Reserved;
        ReservationId = reservationId;
    }

    public void Release(ReservationId reservationId)
    {
        Rule.Check(new TimeslotMustBeReservedByReservation(this, reservationId));

        Status = TimeslotStatus.Available;
        ReservationId = null;
    }

    public void Block()
    {
        Rule.Check(new TimeslotMustNotBeReserved(this));

        Status = TimeslotStatus.Blocked;
    }
}
