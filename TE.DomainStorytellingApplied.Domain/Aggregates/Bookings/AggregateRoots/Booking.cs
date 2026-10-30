
using TE.DomainStorytellingApplied.Domain.Aggregates.Bookers.ValueObjects;
using TE.DomainStorytellingApplied.Domain.Aggregates.Bookings.ValueObjects;
using TE.DomainStorytellingApplied.Domain.Aggregates.Venues.ValueObjects;

namespace TE.DomainStorytellingApplied.Domain.Aggregates.Bookings.AggregateRoots;

public sealed class Booking : IEquatable<Booking>
{
    public Booking(BookingId id, BookerId bookerId, VenueId venueId, TimeSlot timeSlot)
    {
        ArgumentNullException.ThrowIfNull(id);
        ArgumentNullException.ThrowIfNull(bookerId);
        ArgumentNullException.ThrowIfNull(venueId);
        ArgumentNullException.ThrowIfNull(timeSlot);

        Id = id;
        BookerId = bookerId;
        VenueId = venueId;
        TimeSlot = timeSlot;
        Status = BookingStatus.Pending;
    }

    public BookingId Id { get; }

    public BookerId BookerId { get; }

    public VenueId VenueId { get; }

    public TimeSlot TimeSlot { get; }

    public BookingStatus Status { get; private set; }

    public void Confirm()
    {
        if (Status == BookingStatus.Confirmed)
        {
            throw new InvalidOperationException("Booking is already confirmed.");
        }

        Status = BookingStatus.Confirmed;
    }

    public bool Equals(Booking? other)
    {
        return other is not null && Id == other.Id;
    }

    public override bool Equals(object? obj)
    {
        return obj is Booking other && Equals(other);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}
