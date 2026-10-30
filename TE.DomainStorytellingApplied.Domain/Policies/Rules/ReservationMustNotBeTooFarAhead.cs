using TE.DomainStorytellingApplied.Domain.Shared;
using TE.DomainStorytellingApplied.Domain.Timeslots;

namespace TE.DomainStorytellingApplied.Domain.Policies.Rules;

public sealed class ReservationMustNotBeTooFarAhead(
    IReadOnlyCollection<Timeslot> timeslots,
    DateTime nowUtc,
    TimeSpan maximumAdvance) : IValidationRule
{
    public string Domain => nameof(BookingPolicy);

    public string Message => "A reservation cannot be made more than one year ahead.";

    public bool RuleIsBroken => timeslots.Any(t => t.Period.StartUtc > nowUtc + maximumAdvance);
}
