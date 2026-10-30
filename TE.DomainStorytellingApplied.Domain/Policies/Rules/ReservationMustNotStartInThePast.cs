using TE.DomainStorytellingApplied.Domain.Shared;
using TE.DomainStorytellingApplied.Domain.Timeslots;

namespace TE.DomainStorytellingApplied.Domain.Policies.Rules;

public sealed class ReservationMustNotStartInThePast(IReadOnlyCollection<Timeslot> timeslots, DateTime nowUtc) : IValidationRule
{
    public string Domain => nameof(BookingPolicy);

    public string Message => "A reservation cannot start in the past.";

    public bool RuleIsBroken => timeslots.Any(t => t.Period.StartUtc < nowUtc);
}
