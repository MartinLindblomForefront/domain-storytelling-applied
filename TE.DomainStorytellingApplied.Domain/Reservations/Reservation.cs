using TE.DomainStorytellingApplied.Domain.Approvers;
using TE.DomainStorytellingApplied.Domain.Bookers;
using TE.DomainStorytellingApplied.Domain.Premises;

namespace TE.DomainStorytellingApplied.Domain.Reservations;

public sealed class Reservation
{
    private Reservation(
        ReservationId id,
        BookerId bookerId,
        PremisesId premisesId,
        TimeSlot timeSlot,
        ReservationStatus status,
        ApproverId? approverId)
    {
        Id = id;
        BookerId = bookerId;
        PremisesId = premisesId;
        TimeSlot = timeSlot;
        Status = status;
        ApproverId = approverId;
    }

    public ReservationId Id { get; }
    public BookerId BookerId { get; }
    public PremisesId PremisesId { get; }
    public TimeSlot TimeSlot { get; }
    public ReservationStatus Status { get; private set; }
    public ApproverId? ApproverId { get; private set; }

    public static Reservation Request(
        ReservationId id,
        BookerId bookerId,
        PremisesId premisesId,
        TimeSlot timeSlot,
        DateOnly requestedOn,
        IEnumerable<Reservation> existingReservations)
    {
        ArgumentNullException.ThrowIfNull(id);
        ArgumentNullException.ThrowIfNull(bookerId);
        ArgumentNullException.ThrowIfNull(premisesId);
        ArgumentNullException.ThrowIfNull(timeSlot);

        BookingPolicy.EnsureAvailable(premisesId, timeSlot, existingReservations);

        var approvalMode = BookingPolicy.DetermineApprovalMode(
            requestedOn,
            timeSlot);
        var status = approvalMode switch
        {
            ApprovalMode.Automatic => ReservationStatus.Confirmed,
            ApprovalMode.ManualReview => ReservationStatus.PendingApproval,
            _ => throw new ArgumentOutOfRangeException(nameof(approvalMode))
        };

        return new Reservation(
            id,
            bookerId,
            premisesId,
            timeSlot,
            status,
            approverId: null);
    }

    public void Approve(ApproverId approverId)
    {
        ArgumentNullException.ThrowIfNull(approverId);
        EnsurePendingApproval();

        Status = ReservationStatus.Confirmed;
        ApproverId = approverId;
    }

    public void Reject(ApproverId approverId)
    {
        ArgumentNullException.ThrowIfNull(approverId);
        EnsurePendingApproval();

        Status = ReservationStatus.Rejected;
        ApproverId = approverId;
    }

    public void Cancel(DateTimeOffset cancellationRequestedAt)
    {
        if (Status is not (ReservationStatus.PendingApproval or ReservationStatus.Confirmed))
        {
            throw new InvalidOperationException(
                "Bara väntande eller bekräftade bokningar kan avbokas.");
        }

        if (!SwedishBusinessCalendar.IsCancellationAllowed(
                TimeSlot.StartsAt,
                cancellationRequestedAt))
        {
            throw new InvalidOperationException(
                "Bokningen måste avbokas före kl. 12 två arbetsdagar innan start.");
        }

        Status = ReservationStatus.Cancelled;
    }

    private void EnsurePendingApproval()
    {
        if (Status != ReservationStatus.PendingApproval)
        {
            throw new InvalidOperationException(
                "Bara bokningar som väntar på godkännande kan godkännas eller avslås.");
        }
    }
}