using TE.DomainStorytellingApplied.Domain.Aggregates.Bookers.ValueObjects;
using TE.DomainStorytellingApplied.Domain.Aggregates.Bookings.AggregateRoots;
using TE.DomainStorytellingApplied.Domain.Aggregates.Bookings.ValueObjects;
using TE.DomainStorytellingApplied.Domain.Aggregates.Venues.ValueObjects;

namespace TE.DomainStorytellingApplied.Domain.Tests.Aggregates.Bookings.AggregateRoots;

public class BookingTests
{
    [Fact]
    public void GivenValidDetails_WhenCreatingBooking_ShouldCreatePendingBooking()
    {
        var id = new BookingId(Guid.NewGuid());
        var bookerId = new BookerId(Guid.NewGuid());
        var venueId = new VenueId(Guid.NewGuid());
        var timeSlot = CreateTimeSlot();

        var booking = new Booking(id, bookerId, venueId, timeSlot);

        booking.Id.ShouldBe(id);
        booking.BookerId.ShouldBe(bookerId);
        booking.VenueId.ShouldBe(venueId);
        booking.TimeSlot.ShouldBe(timeSlot);
        booking.Status.ShouldBe(BookingStatus.Pending);
    }

    [Fact]
    public void GivenPendingBooking_WhenConfirming_ShouldBecomeConfirmed()
    {
        var booking = CreateBooking();

        booking.Confirm();

        booking.Status.ShouldBe(BookingStatus.Confirmed);
    }

    [Fact]
    public void GivenConfirmedBooking_WhenConfirmingAgain_ShouldThrowInvalidOperationException()
    {
        var booking = CreateBooking();
        booking.Confirm();

        var exception = Should.Throw<InvalidOperationException>(() => booking.Confirm());

        exception.Message.ShouldBe("Booking is already confirmed.");
    }

    [Fact]
    public void GivenSameBookingId_WhenComparingBookings_ShouldBeEqual()
    {
        var id = new BookingId(Guid.NewGuid());
        var first = CreateBooking(id);
        var second = CreateBooking(id);
        second.Confirm();

        first.ShouldBe(second);
    }

    [Fact]
    public void GivenDifferentBookingIds_WhenComparingBookings_ShouldNotBeEqual()
    {
        var first = CreateBooking();
        var second = CreateBooking();

        first.ShouldNotBe(second);
    }

    private static Booking CreateBooking(BookingId? id = null)
    {
        return new Booking(
            id ?? new BookingId(Guid.NewGuid()),
            new BookerId(Guid.NewGuid()),
            new VenueId(Guid.NewGuid()),
            CreateTimeSlot());
    }

    private static TimeSlot CreateTimeSlot()
    {
        var start = new DateTimeOffset(2026, 10, 12, 10, 0, 0, TimeSpan.Zero);
        return new TimeSlot(start, start.AddHours(1));
    }
}
