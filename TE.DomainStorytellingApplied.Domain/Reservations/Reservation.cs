using TE.DomainStorytellingApplied.Domain.Common;

namespace TE.DomainStorytellingApplied.Domain.Reservations;

public class Reservation
{
    public Guid Id { get; private init; }
    public Guid BookerId { get; private init; }
    public Guid TimeslotId { get; private init; }
    public Guid PremiseId { get; private init; }
    public ReservationStatus Status { get; private set; }

    public Reservation(Guid bookerId, Guid timeslotId, Guid premiseId)
    {
        Guard.EnsureNotEmpty(bookerId);
        Guard.EnsureNotEmpty(timeslotId);
        Guard.EnsureNotEmpty(premiseId);

        Id = Guid.NewGuid();
        BookerId = bookerId;
        TimeslotId = timeslotId;
        PremiseId = premiseId;
        Status = ReservationStatus.Pending;
    }
}
