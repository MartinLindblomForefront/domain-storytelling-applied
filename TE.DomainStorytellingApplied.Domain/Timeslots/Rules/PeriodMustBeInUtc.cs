using TE.DomainStorytellingApplied.Domain.Shared;

namespace TE.DomainStorytellingApplied.Domain.Timeslots.Rules;

public sealed class PeriodMustBeInUtc(DateTime startUtc, DateTime endUtc) : IValidationRule
{
    public string Domain => nameof(Period);

    public string Message => "Period must be expressed in UTC.";

    public bool RuleIsBroken => startUtc.Kind != DateTimeKind.Utc || endUtc.Kind != DateTimeKind.Utc;
}
