using TE.DomainStorytellingApplied.Domain.Aggregates.Bookings.ValueObjects;

namespace TE.DomainStorytellingApplied.Domain.Tests.Aggregates.Bookings.ValueObjects;

public class BookingIdTests
{
    [Fact]
    public void GivenValidGuid_WhenCreatingBookingId_ShouldContainGuid()
    {
        var value = Guid.NewGuid();

        var bookingId = new BookingId(value);

        bookingId.Value.ShouldBe(value);
    }

    [Fact]
    public void GivenEmptyGuid_WhenCreatingBookingId_ShouldThrowArgumentException()
    {
        var exception = Should.Throw<ArgumentException>(() => new BookingId(Guid.Empty));

        exception.Message.ShouldContain("Booking ID cannot be empty.");
        exception.ParamName.ShouldBe("value");
    }

    [Fact]
    public void GivenSameGuid_WhenComparingBookingIds_ShouldBeEqual()
    {
        var value = Guid.NewGuid();
        var first = new BookingId(value);
        var second = new BookingId(value);

        first.ShouldBe(second);
    }

    [Fact]
    public void GivenDifferentGuids_WhenComparingBookingIds_ShouldNotBeEqual()
    {
        var first = new BookingId(Guid.NewGuid());
        var second = new BookingId(Guid.NewGuid());

        first.ShouldNotBe(second);
    }
}
