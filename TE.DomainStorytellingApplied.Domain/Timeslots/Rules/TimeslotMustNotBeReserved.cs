using TE.DomainStorytellingApplied.Domain.Shared;

namespace TE.DomainStorytellingApplied.Domain.Timeslots.Rules;

public sealed class TimeslotMustNotBeReserved(Timeslot timeslot) : IValidationRule
{
    public string Domain => nameof(Timeslot);

    public string Message => "Reserved timeslot must be released before it is blocked.";

    public bool RuleIsBroken => timeslot.Status == TimeslotStatus.Reserved;
}
