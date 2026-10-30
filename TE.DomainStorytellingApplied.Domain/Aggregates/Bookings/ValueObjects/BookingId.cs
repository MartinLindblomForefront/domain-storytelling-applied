
namespace TE.DomainStorytellingApplied.Domain.Aggregates.Bookings.ValueObjects;

public sealed record BookingId
{
    public BookingId(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException("Booking ID cannot be empty.", nameof(value));
        }

        Value = value;
    }

    public Guid Value { get; }
}
