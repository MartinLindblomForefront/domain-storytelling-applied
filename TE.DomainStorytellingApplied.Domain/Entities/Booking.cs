namespace TE.DomainStorytellingApplied.Domain;

/// <summary>
/// An entity: one booking of a venue for a period of time.
/// A booking can only be created through Reserve, which checks the venue's rules first.
/// It remembers the venue by its Id and keeps its own copy of the fee.
/// </summary>
public sealed class Booking
{
    // How long a new booking holds its time before it must be paid.
    private static readonly TimeSpan ReservationHoldTime = TimeSpan.FromMinutes(15);

    // Cancelling a confirmed booking closer to the start than this counts as late.
    private static readonly TimeSpan CancellationDeadline = TimeSpan.FromHours(48);

    // Things that have happened to this booking. They are only recorded here.
    // Reacting to them (for example sending an email) happens outside the domain.
    private readonly List<IDomainEvent> _domainEvents = new();

    // Private setters: a booking can change over time, but only through its own methods.
    public Guid Id { get; private set; }

    public Guid VenueId { get; private set; }

    // Kept so the fee can be worked out again if the booking is moved to another time.
    public BookerType BookerType { get; private set; }

    public TimeSlot TimeSlot { get; private set; }

    public int ParticipantCount { get; private set; }

    public ContactDetails Contact { get; private set; }

    public Money Fee { get; private set; }

    public BookingStatus Status { get; private set; }

    public DateTime ReservedUntil { get; private set; }

    // Null until the booking is paid.
    public Payment? Payment { get; private set; }

    // True when a confirmed booking was cancelled less than 48 hours before it starts.
    public bool IsLateCancellation { get; private set; }

    // A reserved or confirmed booking holds its time. A cancelled or expired one has given it back.
    public bool IsHoldingItsTime => Status == BookingStatus.Reserved || Status == BookingStatus.Confirmed;

    // Read-only from outside: only the booking itself can record an event.
    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents;

    // Private: the only way to create a booking from outside is Reserve.
    private Booking(Guid venueId, BookerType bookerType, TimeSlot timeSlot, int participantCount, ContactDetails contact, Money fee, DateTime reservedUntil)
    {
        Id = Guid.NewGuid();
        VenueId = venueId;
        BookerType = bookerType;
        TimeSlot = timeSlot;
        ParticipantCount = participantCount;
        Contact = contact;
        Fee = fee;
        Status = BookingStatus.Reserved;
        ReservedUntil = reservedUntil;
    }

    public static Booking Reserve(
        Venue venue,
        BookerType bookerType,
        TimeSlot timeSlot,
        int participantCount,
        ContactDetails contact,
        DateTime now)
    {
        if (!venue.CanBeBookedBy(bookerType))
        {
            throw new ArgumentException($"{venue.Name} cannot be booked by this type of booker.", nameof(bookerType));
        }

        if (!venue.IsOpenDuring(timeSlot))
        {
            throw new ArgumentException($"{venue.Name} is closed at that time.", nameof(timeSlot));
        }

        if (participantCount < 1)
        {
            throw new ArgumentException("A booking needs at least one participant.", nameof(participantCount));
        }

        if (participantCount > venue.Capacity)
        {
            throw new ArgumentException($"{venue.Name} allows a maximum of {venue.Capacity} people.", nameof(participantCount));
        }

        // The fee is copied, so a later price change on the venue does not touch this booking.
        var fee = venue.CalculateFee(timeSlot, bookerType);

        return new Booking(venue.Id, bookerType, timeSlot, participantCount, contact, fee, now + ReservationHoldTime);
    }

    // Paying confirms the booking. The current time is passed in, so the rule can be tested without waiting.
    public void Pay(Payment payment, DateTime now)
    {
        if (Status != BookingStatus.Reserved)
        {
            throw new InvalidOperationException("Only a reserved booking can be paid.");
        }

        if (now > ReservedUntil)
        {
            throw new InvalidOperationException("The reservation has run out.");
        }

        if (payment.Amount != Fee)
        {
            throw new ArgumentException("The amount does not match the fee.", nameof(payment));
        }

        Payment = payment;
        Status = BookingStatus.Confirmed;

        _domainEvents.Add(new BookingConfirmed(Id, Contact.Email, Fee));
    }

    // A late cancellation is allowed. The booking only records that it was late.
    // What that costs the booker is decided somewhere else.
    public void Cancel(DateTime now)
    {
        if (Status == BookingStatus.Cancelled || Status == BookingStatus.Expired)
        {
            throw new InvalidOperationException("The booking is already closed.");
        }

        var timeUntilStart = TimeSlot.Start - now;

        IsLateCancellation = Status == BookingStatus.Confirmed && timeUntilStart < CancellationDeadline;
        Status = BookingStatus.Cancelled;
    }

    // Moves the booking to another time. The time slot is not edited, it is replaced:
    // TimeSlot has no setters, so the booking takes a whole new one.
    // The fee follows the new slot. A paid booking has already been charged,
    // so it can only be moved to a time that costs the same.
    public void ChangeTimeSlot(Venue venue, TimeSlot timeSlot)
    {
        if (Status == BookingStatus.Cancelled || Status == BookingStatus.Expired)
        {
            throw new InvalidOperationException("A closed booking cannot be moved.");
        }

        if (venue.Id != VenueId)
        {
            throw new ArgumentException("That is not the venue of this booking.", nameof(venue));
        }

        if (!venue.IsOpenDuring(timeSlot))
        {
            throw new ArgumentException($"{venue.Name} is closed at that time.", nameof(timeSlot));
        }

        var fee = venue.CalculateFee(timeSlot, BookerType);

        if (Status == BookingStatus.Confirmed && fee != Fee)
        {
            throw new ArgumentException("A paid booking can only be moved to a time that costs the same.", nameof(timeSlot));
        }

        TimeSlot = timeSlot;
        Fee = fee;
    }

    // Releases a reservation that was not paid in time. For any other booking it does nothing,
    // so it is safe to call on every booking without checking first.
    public void Expire(DateTime now)
    {
        if (Status == BookingStatus.Reserved && now > ReservedUntil)
        {
            Status = BookingStatus.Expired;
        }
    }
}
