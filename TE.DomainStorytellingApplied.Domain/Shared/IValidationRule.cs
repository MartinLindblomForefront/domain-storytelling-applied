namespace TE.DomainStorytellingApplied.Domain.Shared;

public interface IValidationRule
{
    string Domain { get; }

    string Message { get; }

    bool RuleIsBroken { get; }
}
