using TE.DomainStorytellingApplied.Domain.Shared;
using TE.DomainStorytellingApplied.Domain.Timeslots;

namespace TE.DomainStorytellingApplied.Domain.Policies.Rules;

public sealed class TimeslotsMustBeInSamePremises(IReadOnlyCollection<Timeslot> timeslots) : IValidationRule
{
    public string Domain => nameof(BookingPolicy);

    public string Message => "All timeslots must be in the same premises.";

    public bool RuleIsBroken => timeslots.Select(t => t.PremisesId).Distinct().Count() > 1;
}
