namespace TE.DomainStorytellingApplied.Domain.Tests;

public class BookingTests
{
    private static Booking CreatePendingBooking()
    {
        var startTime = new DateTimeOffset(2026, 10, 9, 8, 0, 0, TimeSpan.Zero);

        return new Booking(
            Id.Generate(),
            Id.Generate(),
            new Booker("Anna", BookerType.Private),
            new Timeslot(startTime, startTime.AddHours(1)));
    }

    [Fact]
    public void GivenNewBooking_WhenCreated_ShouldBePending()
    {
        var booking = CreatePendingBooking();

        booking.Status.ShouldBe(BookingStatus.Pending);
    }

    [Fact]
    public void GivenPendingBooking_WhenConfirming_ShouldBeConfirmed()
    {
        var booking = CreatePendingBooking();

        booking.Confirm();

        booking.Status.ShouldBe(BookingStatus.Confirmed);
    }

    [Fact]
    public void GivenConfirmedBooking_WhenConfirming_ShouldThrow()
    {
        var booking = CreatePendingBooking();
        booking.Confirm();

        Should.Throw<Exception>(() => booking.Confirm());
    }

    [Fact]
    public void GivenPendingBooking_WhenCancelling_ShouldBeCancelled()
    {
        var booking = CreatePendingBooking();

        booking.Cancel();

        booking.Status.ShouldBe(BookingStatus.Cancelled);
    }

    [Fact]
    public void GivenCancelledBooking_WhenConfirming_ShouldThrow()
    {
        var booking = CreatePendingBooking();
        booking.Cancel();

        Should.Throw<Exception>(() => booking.Confirm());
    }

    [Fact]
    public void GivenConfirmedBooking_WhenCancelling_ShouldThrow()
    {
        var booking = CreatePendingBooking();
        booking.Confirm();

        Should.Throw<Exception>(() => booking.Cancel());
    }
}
