using static TE.DomainStorytellingApplied.Domain.Tests.TestData;

namespace TE.DomainStorytellingApplied.Domain.Tests;

public class ReservationTimeoutTests
{
    [Fact]
    public void GivenReservationTimedOut_WhenAnotherReserves_ShouldSucceed()
    {
        var venue = NewVenue();
        venue.Reserve(Tomorrow18, BookerType.Association, Now);

        Should.NotThrow(() => venue.Reserve(Tomorrow18, BookerType.PrivatePerson, AfterTimeout(Now)));
    }

    [Fact]
    public void GivenConfirmedBooking_WhenTimeoutPasses_ShouldStillBlockSlot()
    {
        var venue = NewVenue();
        venue.Reserve(Tomorrow18, BookerType.Municipality, Now);

        venue.IsAvailable(Tomorrow18, AfterTimeout(Now)).ShouldBeFalse();
    }
}
