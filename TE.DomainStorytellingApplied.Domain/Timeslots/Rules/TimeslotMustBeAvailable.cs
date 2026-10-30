using TE.DomainStorytellingApplied.Domain.Shared;

namespace TE.DomainStorytellingApplied.Domain.Timeslots.Rules;

public sealed class TimeslotMustBeAvailable(Timeslot timeslot) : IValidationRule
{
    public string Domain => nameof(Timeslot);

    public string Message => "Timeslot is not available.";

    public bool RuleIsBroken => timeslot.Status != TimeslotStatus.Available;
}
