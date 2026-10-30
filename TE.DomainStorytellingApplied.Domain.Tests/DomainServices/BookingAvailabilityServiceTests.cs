using TE.DomainStorytellingApplied.Domain.Aggregates.Bookers.ValueObjects;
using TE.DomainStorytellingApplied.Domain.Aggregates.Bookings.AggregateRoots;
using TE.DomainStorytellingApplied.Domain.Aggregates.Bookings.ValueObjects;
using TE.DomainStorytellingApplied.Domain.Aggregates.Venues.ValueObjects;
using TE.DomainStorytellingApplied.Domain.DomainServices;

namespace TE.DomainStorytellingApplied.Domain.Tests.DomainServices;

public class BookingAvailabilityServiceTests
{
    private readonly BookingAvailabilityService service = new();

    [Fact]
    public void GivenNoExistingBookings_WhenCheckingAvailability_ShouldBeAvailable()
    {
        var venueId = new VenueId(Guid.NewGuid());
        var requestedTimeSlot = CreateTimeSlot(10, 11);

        var isAvailable = service.IsAvailable(venueId, requestedTimeSlot, []);

        isAvailable.ShouldBeTrue();
    }

    [Fact]
    public void GivenOverlappingBookingForAnotherVenue_WhenCheckingAvailability_ShouldBeAvailable()
    {
        var requestedVenueId = new VenueId(Guid.NewGuid());
        var existingBooking = CreateBooking(new VenueId(Guid.NewGuid()), CreateTimeSlot(10, 11));

        var isAvailable = service.IsAvailable(
            requestedVenueId,
            CreateTimeSlot(10, 11),
            [existingBooking]);

        isAvailable.ShouldBeTrue();
    }

    [Fact]
    public void GivenNonOverlappingBookingForSameVenue_WhenCheckingAvailability_ShouldBeAvailable()
    {
        var venueId = new VenueId(Guid.NewGuid());
        var existingBooking = CreateBooking(venueId, CreateTimeSlot(10, 11));

        var isAvailable = service.IsAvailable(
            venueId,
            CreateTimeSlot(11, 12),
            [existingBooking]);

        isAvailable.ShouldBeTrue();
    }

    [Fact]
    public void GivenOverlappingBookingForSameVenue_WhenCheckingAvailability_ShouldNotBeAvailable()
    {
        var venueId = new VenueId(Guid.NewGuid());
        var existingBooking = CreateBooking(venueId, CreateTimeSlot(10, 11));

        var isAvailable = service.IsAvailable(
            venueId,
            CreateTimeSlot(10, 12),
            [existingBooking]);

        isAvailable.ShouldBeFalse();
    }

    private static Booking CreateBooking(VenueId venueId, TimeSlot timeSlot)
    {
        return new Booking(
            new BookingId(Guid.NewGuid()),
            new BookerId(Guid.NewGuid()),
            venueId,
            timeSlot);
    }

    private static TimeSlot CreateTimeSlot(int startHour, int endHour)
    {
        var date = new DateTimeOffset(2026, 10, 12, 0, 0, 0, TimeSpan.Zero);
        return new TimeSlot(date.AddHours(startHour), date.AddHours(endHour));
    }
}
