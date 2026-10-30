using TE.DomainStorytellingApplied.Domain.Shared;

namespace TE.DomainStorytellingApplied.Domain.Policies.Rules;

public sealed class TotalDurationMustBeAtLeast(TimeSpan totalDuration, TimeSpan minimumDuration) : IValidationRule
{
    public string Domain => nameof(BookingPolicy);

    public string Message => $"A reservation must be at least {minimumDuration.TotalHours} hour(s).";

    public bool RuleIsBroken => totalDuration < minimumDuration;
}
