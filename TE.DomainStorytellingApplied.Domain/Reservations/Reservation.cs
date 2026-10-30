using TE.DomainStorytellingApplied.Domain.Bookers;
using TE.DomainStorytellingApplied.Domain.Premises;
using TE.DomainStorytellingApplied.Domain.Timeslots;

namespace TE.DomainStorytellingApplied.Domain.Reservations;

public class Reservation
{
    public Guid Id { get; private init; }
    public Booker Booker { get; private init; }
    public Timeslot Timeslot { get; private init; }
    public Premise Premise { get; private init; }
    public ReservationStatus Status { get; private set; }

    public Reservation(Booker booker, Timeslot timeslot, Premise premise)
    {
        ArgumentNullException.ThrowIfNull(booker);
        ArgumentNullException.ThrowIfNull(timeslot);
        ArgumentNullException.ThrowIfNull(premise);

        if (timeslot.PremisesId != premise.Id)
        {
            throw new ArgumentException("The timeslot does not belong to the given premise.", nameof(timeslot));
        }

        Id = Guid.NewGuid();
        Booker = booker;
        Timeslot = timeslot;
        Premise = premise;
        Status = ReservationStatus.Pending;
    }
}
