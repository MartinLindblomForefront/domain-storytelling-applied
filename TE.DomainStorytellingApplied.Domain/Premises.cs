namespace TE.DomainStorytellingApplied.Domain;

public sealed class Premises
{
    private readonly List<Reservation> _reservations = [];

    public Premises(string name, BookingPolicy bookingPolicy)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(bookingPolicy);

        Id = Guid.NewGuid();
        Name = name;
        BookingPolicy = bookingPolicy;
    }

    public Guid Id { get; }

    public string Name { get; }

    public BookingPolicy BookingPolicy { get; }

    public IReadOnlyCollection<Reservation> Reservations => _reservations.AsReadOnly();

    public Reservation Reserve(Booker booker, Timeslot timeslot, DateTimeOffset now)
    {
        ArgumentNullException.ThrowIfNull(booker);
        ArgumentNullException.ThrowIfNull(timeslot);

        if (!BookingPolicy.Allows(booker, timeslot, now))
        {
            throw new InvalidOperationException("The booking policy does not allow this reservation.");
        }

        if (_reservations.Any(r => r.Status != ReservationStatus.Rejected && r.Timeslot.Overlaps(timeslot)))
        {
            throw new InvalidOperationException("The timeslot overlaps an existing reservation.");
        }

        var reservation = new Reservation(booker.Id, Id, timeslot);
        reservation.Submit(BookingPolicy.RequiresReview);
        _reservations.Add(reservation);

        return reservation;
    }

    public void Approve(Guid reservationId, Approver approver) =>
        FindReservation(reservationId).Approve(approver);

    public void Reject(Guid reservationId, Approver approver, string reason) =>
        FindReservation(reservationId).Reject(approver, reason);

    private Reservation FindReservation(Guid reservationId) =>
        _reservations.SingleOrDefault(r => r.Id == reservationId)
        ?? throw new ArgumentException("No reservation with the given id exists.", nameof(reservationId));
}
