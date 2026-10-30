using static TE.DomainStorytellingApplied.Domain.Tests.TestData;

namespace TE.DomainStorytellingApplied.Domain.Tests;

public class ConfirmBookingTests
{
    [Fact]
    public void GivenPaymentInTime_WhenConfirming_ShouldConfirm()
    {
        var venue = NewVenue();
        var booking = venue.Reserve(Tomorrow18, BookerType.Association, Now);

        venue.ConfirmBooking(booking.Id, Now.AddMinutes(5));

        booking.Status.ShouldBe(BookingStatus.Confirmed);
    }

    [Fact]
    public void GivenPaymentAfterTimeout_WhenConfirming_ShouldThrow()
    {
        var venue = NewVenue();
        var booking = venue.Reserve(Tomorrow18, BookerType.Association, Now);

        Should.Throw<DomainException>(() => venue.ConfirmBooking(booking.Id, AfterTimeout(Now)));
    }
}
