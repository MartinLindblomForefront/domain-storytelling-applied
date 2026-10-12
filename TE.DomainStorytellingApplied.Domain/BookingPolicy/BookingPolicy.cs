namespace TE.DomainStorytellingApplied.Domain;

public sealed class BookingPolicy(
    IReservationRepository reservations,
    ITimeslotRepository timeslots,
    TimeProvider timeProvider,
    MaxActiveReservations maxActiveReservations)
{
    public IReadOnlyList<PolicyViolation> Check(Booker booker, Id timeslotId)
    {
        var violations = new List<PolicyViolation>();

        var timeslot = timeslots.Find(timeslotId);
        if (timeslot is null)
        {
            violations.Add(PolicyViolation.TimeslotNotFound);
        }
        else
        {
            if (!timeslot.IsInFutureOf(timeProvider.GetUtcNow()))
            {
                violations.Add(PolicyViolation.TimeslotInPast);
            }

            if (timeslot.IsReserved)
            {
                violations.Add(PolicyViolation.TimeslotAlreadyReserved);
            }
        }

        if (reservations.CountActiveFor(booker.Id) >= maxActiveReservations.Value)
        {
            violations.Add(PolicyViolation.MaxActiveReservationsReached);
        }

        return violations;
    }

    public void Enforce(Booker booker, Id timeslotId)
    {
        var violations = Check(booker, timeslotId);
        if (violations.Count > 0)
        {
            throw new BookingPolicyViolationException(violations);
        }
    }
}

public sealed class BookingPolicyViolationException(IReadOnlyList<PolicyViolation> violations)
    : InvalidOperationException("Booking policy violated: " + string.Join(", ", violations.Select(v => v.Code)))
{
    public IReadOnlyList<PolicyViolation> Violations { get; } = violations;
}
