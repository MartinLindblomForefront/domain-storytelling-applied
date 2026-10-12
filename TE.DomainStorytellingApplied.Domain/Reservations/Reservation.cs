namespace TE.DomainStorytellingApplied.Domain;

public sealed class Reservation
{
    public Id Id { get; }
    public Id BookerId { get; }
    public Id PremisesId { get; }
    public Id TimeslotId { get; }
    public ReservationStatus Status { get; private set; } = ReservationStatus.PendingReview;
    public Id? ReviewedBy { get; private set; }

    public bool IsActive => Status is ReservationStatus.PendingReview or ReservationStatus.Approved;

    public Reservation(Id id, Id bookerId, Id premisesId, Id timeslotId)
    {
        Id = id;
        BookerId = bookerId;
        PremisesId = premisesId;
        TimeslotId = timeslotId;
    }

    public void Approve(Id approverId) => Review(approverId, ReservationStatus.Approved);

    public void Reject(Id approverId) => Review(approverId, ReservationStatus.Rejected);

    private void Review(Id approverId, ReservationStatus outcome)
    {
        if (Status != ReservationStatus.PendingReview)
        {
            throw new InvalidOperationException("Reservation has already been reviewed.");
        }

        Status = outcome;
        ReviewedBy = approverId;
    }
}
