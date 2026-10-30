using TE.DomainStorytellingApplied.Domain.Shared;

namespace TE.DomainStorytellingApplied.Domain.Reservations.Rules;

public sealed class RejectionMustHaveReason(string? reason) : IValidationRule
{
    public string Domain => nameof(Reservation);

    public string Message => "A rejection needs a reason.";

    public bool RuleIsBroken => string.IsNullOrWhiteSpace(reason);
}
