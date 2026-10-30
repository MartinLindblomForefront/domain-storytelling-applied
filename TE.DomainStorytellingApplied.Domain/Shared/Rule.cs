namespace TE.DomainStorytellingApplied.Domain.Shared;

public static class Rule
{
    public static void Check(IValidationRule rule)
    {
        if (rule.RuleIsBroken)
            throw new DomainValidationException(rule.Domain, rule.Message);
    }
}
