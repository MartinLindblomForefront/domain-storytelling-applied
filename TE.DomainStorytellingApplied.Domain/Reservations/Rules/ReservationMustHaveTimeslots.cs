using TE.DomainStorytellingApplied.Domain.Shared;
using TE.DomainStorytellingApplied.Domain.Timeslots;

namespace TE.DomainStorytellingApplied.Domain.Reservations.Rules;

public sealed class ReservationMustHaveTimeslots(IReadOnlyCollection<TimeslotId> timeslotIds) : IValidationRule
{
    public string Domain => nameof(Reservation);

    public string Message => "A reservation needs at least one timeslot.";

    public bool RuleIsBroken => timeslotIds.Count == 0;
}
