using TE.DomainStorytellingApplied.Domain.Bookers;
using TE.DomainStorytellingApplied.Domain.Policies;
using TE.DomainStorytellingApplied.Domain.Premises;
using TE.DomainStorytellingApplied.Domain.Timeslots;

namespace TE.DomainStorytellingApplied.Domain.Reservations;

// Domain aggregate root
public sealed class Reservation
{
    public Guid Id { get; }
    public BookerType BookerType { get; }
    public Premise Premise { get; }
    public Timeslot Timeslot { get; }
    public ReservationStatus Status { get; private set; }

    private Reservation(Guid id, BookerType bookerType, Premise premise, Timeslot timeslot)
    {
        Id = id;
        BookerType = bookerType;
        Premise = premise;
        Timeslot = timeslot;
        Status = ReservationStatus.Requested;
    }

    public static Reservation Create(Guid id, BookerType bookerType, Premise premise, Timeslot timeslot)
    {
        if (id == Guid.Empty)
        {
            throw InvalidReservationException.EmptyId();
        }

        ArgumentNullException.ThrowIfNull(premise);
        ArgumentNullException.ThrowIfNull(timeslot);

        return new Reservation(id, bookerType, premise, timeslot);
    }

    public void Evaluate(ApprovalPolicy approvalPolicy)
    {
        ArgumentNullException.ThrowIfNull(approvalPolicy);
        EnsureStatus(ReservationStatus.Requested, nameof(Evaluate));

        Status = approvalPolicy.DetermineRoute(BookerType, Timeslot) switch
        {
            ApprovalRoute.AutoApprove => ReservationStatus.Approved,
            ApprovalRoute.RouteForReview => ReservationStatus.PendingReview,
            var route => throw new ArgumentOutOfRangeException(nameof(approvalPolicy), route, null),
        };
    }

    public void Approve()
    {
        EnsureStatus(ReservationStatus.PendingReview, nameof(Approve));
        Status = ReservationStatus.Approved;
    }

    public void Reject()
    {
        EnsureStatus(ReservationStatus.PendingReview, nameof(Reject));
        Status = ReservationStatus.Rejected;
    }

    private void EnsureStatus(ReservationStatus required, string action)
    {
        if (Status != required)
        {
            throw InvalidReservationTransitionException.Cannot(action, Status);
        }
    }
}
