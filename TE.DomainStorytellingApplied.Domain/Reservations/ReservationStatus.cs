namespace TE.DomainStorytellingApplied.Domain.Reservations;

// Domain value object
public enum ReservationStatus
{
    Requested = 0,
    PendingReview,
    Approved,
    Rejected,
}
