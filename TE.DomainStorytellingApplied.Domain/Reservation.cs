namespace TE.DomainStorytellingApplied.Domain;

public sealed class Reservation
{
    internal Reservation(Guid bookerId, Guid premisesId, Timeslot timeslot)
    {
        if (bookerId == Guid.Empty)
        {
            throw new ArgumentException("Booker id must not be empty.", nameof(bookerId));
        }

        if (premisesId == Guid.Empty)
        {
            throw new ArgumentException("Premises id must not be empty.", nameof(premisesId));
        }

        ArgumentNullException.ThrowIfNull(timeslot);

        Id = Guid.NewGuid();
        BookerId = bookerId;
        PremisesId = premisesId;
        Timeslot = timeslot;
        Status = ReservationStatus.Created;
    }

    public Guid Id { get; }

    public Guid BookerId { get; }

    public Guid PremisesId { get; }

    public Timeslot Timeslot { get; }

    public ReservationStatus Status { get; private set; }

    public Guid? ReviewedById { get; private set; }

    public string? RejectionReason { get; private set; }

    internal void Submit(bool requiresReview)
    {
        EnsureStatus(ReservationStatus.Created);

        Status = requiresReview
            ? ReservationStatus.PendingReview
            : ReservationStatus.Approved;
    }

    internal void Approve(Approver approver)
    {
        ArgumentNullException.ThrowIfNull(approver);
        EnsureStatus(ReservationStatus.PendingReview);

        Status = ReservationStatus.Approved;
        ReviewedById = approver.Id;
    }

    internal void Reject(Approver approver, string reason)
    {
        ArgumentNullException.ThrowIfNull(approver);
        ArgumentException.ThrowIfNullOrWhiteSpace(reason);
        EnsureStatus(ReservationStatus.PendingReview);

        Status = ReservationStatus.Rejected;
        ReviewedById = approver.Id;
        RejectionReason = reason;
    }

    private void EnsureStatus(ReservationStatus expected)
    {
        if (Status != expected)
        {
            throw new InvalidOperationException(
                $"Reservation must have status {expected} but has status {Status}.");
        }
    }
}
