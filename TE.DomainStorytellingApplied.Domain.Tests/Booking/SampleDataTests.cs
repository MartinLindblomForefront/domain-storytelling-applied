using TE.DomainStorytellingApplied.Domain.Booking;
using TE.DomainStorytellingApplied.Domain.Reservations;

namespace TE.DomainStorytellingApplied.Domain.Tests.Booking;

public class SampleDataTests
{
    [Fact]
    public void GivenNewSystem_WhenAddingSampleData_ShouldPopulateAllLists()
    {
        var system = new BookingSystem().WithSampleData();

        system.Premises.Count.ShouldBe(3);
        system.Bookers.Count.ShouldBe(3);
        system.Timeslots.Count.ShouldBe(5);
        system.Reservations.Count.ShouldBe(3);
        system.Reservations.ShouldAllBe(r => r.Status == ReservationStatus.Pending);
    }
}
