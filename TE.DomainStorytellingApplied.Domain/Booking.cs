namespace TE.DomainStorytellingApplied.Domain;

public class Booking
{
    public Id BookingId { get; init; }
    public Id TimeslotId { get; init; }
    public Id BookerId { get; init; }

    public Booking(Id bookingId, Id timeslotId, Id bookerId)
    {
        BookingId = bookingId;
        TimeslotId = timeslotId;
        BookerId = bookerId;
    }

    public static Booking Book(Id timeslotId, Id bookerId) =>
        new(Id.Generate(), timeslotId, bookerId);
}