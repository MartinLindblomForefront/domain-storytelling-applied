using TE.DomainStorytellingApplied.Domain.Shared;

namespace TE.DomainStorytellingApplied.Domain.Timeslots.Rules;

public sealed class PeriodMustEndAfterItStarts(DateTime startUtc, DateTime endUtc) : IValidationRule
{
    public string Domain => nameof(Period);

    public string Message => "End must be after start.";

    public bool RuleIsBroken => endUtc <= startUtc;
}
