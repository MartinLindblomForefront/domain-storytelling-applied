
using TE.DomainStorytellingApplied.Domain.Aggregates.Bookings.AggregateRoots;
using TE.DomainStorytellingApplied.Domain.Aggregates.Bookings.ValueObjects;
using TE.DomainStorytellingApplied.Domain.Aggregates.Venues.ValueObjects;

namespace TE.DomainStorytellingApplied.Domain.DomainServices;

public sealed class BookingAvailabilityService
{
    public bool IsAvailable(
        VenueId venueId,
        TimeSlot requestedTimeSlot,
        IEnumerable<Booking> existingBookings)
    {
        ArgumentNullException.ThrowIfNull(venueId);
        ArgumentNullException.ThrowIfNull(requestedTimeSlot);
        ArgumentNullException.ThrowIfNull(existingBookings);

        return !existingBookings.Any(booking =>
            booking.VenueId == venueId &&
            booking.TimeSlot.OverlapsWith(requestedTimeSlot));
    }
}
