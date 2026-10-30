using TE.DomainStorytellingApplied.Domain.Reservations;
using TE.DomainStorytellingApplied.Domain.Shared;

namespace TE.DomainStorytellingApplied.Domain.Timeslots.Rules;

public sealed class TimeslotMustBeReservedByReservation(Timeslot timeslot, ReservationId reservationId) : IValidationRule
{
    public string Domain => nameof(Timeslot);

    public string Message => "Timeslot is not reserved by this reservation.";

    public bool RuleIsBroken => timeslot.Status != TimeslotStatus.Reserved || timeslot.ReservationId != reservationId;
}
