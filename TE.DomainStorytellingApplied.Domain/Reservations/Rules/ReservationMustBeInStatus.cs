using TE.DomainStorytellingApplied.Domain.Shared;

namespace TE.DomainStorytellingApplied.Domain.Reservations.Rules;

public sealed class ReservationMustBeInStatus(
    Reservation reservation,
    ReservationStatus[] allowedStatuses,
    ReservationStatus targetStatus) : IValidationRule
{
    public string Domain => nameof(Reservation);

    public string Message => $"A reservation that is {reservation.Status} cannot become {targetStatus}.";

    public bool RuleIsBroken => !allowedStatuses.Contains(reservation.Status);
}
