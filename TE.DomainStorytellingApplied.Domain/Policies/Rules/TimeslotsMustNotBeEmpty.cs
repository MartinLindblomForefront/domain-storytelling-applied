using TE.DomainStorytellingApplied.Domain.Shared;
using TE.DomainStorytellingApplied.Domain.Timeslots;

namespace TE.DomainStorytellingApplied.Domain.Policies.Rules;

public sealed class TimeslotsMustNotBeEmpty(IReadOnlyCollection<Timeslot> timeslots) : IValidationRule
{
    public string Domain => nameof(BookingPolicy);

    public string Message => "No timeslots were selected.";

    public bool RuleIsBroken => timeslots.Count == 0;
}
