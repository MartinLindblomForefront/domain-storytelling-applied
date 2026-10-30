using TE.DomainStorytellingApplied.Domain.Reservations.Rules;
using TE.DomainStorytellingApplied.Domain.Shared;
using TE.DomainStorytellingApplied.Domain.Timeslots;

namespace TE.DomainStorytellingApplied.Domain.Reservations;

public sealed class Reservation
{
    private readonly List<TimeslotId> _timeslotIds;

    private Reservation(ReservationId id, BookerId bookerId, PremisesId premisesId, List<TimeslotId> timeslotIds)
    {
        Id = id;
        BookerId = bookerId;
        PremisesId = premisesId;
        _timeslotIds = timeslotIds;
        Status = ReservationStatus.Requested;
    }

    public ReservationId Id { get; }

    public BookerId BookerId { get; }

    public PremisesId PremisesId { get; }

    public IReadOnlyList<TimeslotId> TimeslotIds => _timeslotIds;

    public ReservationStatus Status { get; private set; }

    public string? RejectionReason { get; private set; }

    public static Reservation Request(
        BookerId bookerId, PremisesId premisesId, IEnumerable<TimeslotId> timeslotIds)
    {
        var ids = timeslotIds.Distinct().ToList();
        Rule.Check(new ReservationMustHaveTimeslots(ids));

        return new Reservation(ReservationId.New(), bookerId, premisesId, ids);
    }

    public void SendToReview() =>
        MoveFrom([ReservationStatus.Requested], ReservationStatus.UnderReview);

    public void Approve() =>
        MoveFrom([ReservationStatus.Requested, ReservationStatus.UnderReview], ReservationStatus.Booked);

    public void Reject(string reason)
    {
        Rule.Check(new RejectionMustHaveReason(reason));

        MoveFrom([ReservationStatus.Requested, ReservationStatus.UnderReview], ReservationStatus.Rejected);
        RejectionReason = reason;
    }

    private void MoveFrom(ReservationStatus[] allowedFrom, ReservationStatus to)
    {
        Rule.Check(new ReservationMustBeInStatus(this, allowedFrom, to));

        Status = to;
    }
}
