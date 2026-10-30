namespace TE.DomainStorytellingApplied.Domain.Tests;

public class VenueAvailabilityServiceTests
{
    private static readonly DateTime Now = TestData.Now;

    // The slot somebody wants to book: 10:00 to 11:00.
    private static readonly TimeSlot RequestedSlot = SlotBetween(10, 11);

    private static TimeSlot SlotBetween(int startHour, int endHour)
    {
        return new TimeSlot(
            new DateTime(2026, 10, 20, startHour, 0, 0, DateTimeKind.Utc),
            new DateTime(2026, 10, 20, endHour, 0, 0, DateTimeKind.Utc));
    }

    [Fact]
    public void GivenNoBookings_WhenCheckingAvailability_ShouldBeAvailable()
    {
        var venue = TestData.CreateVenue();

        VenueAvailabilityService.IsAvailable(venue.Id, RequestedSlot, []).ShouldBeTrue();
    }

    [Fact]
    public void GivenAReservedBookingAtTheSameTime_WhenCheckingAvailability_ShouldNotBeAvailable()
    {
        var venue = TestData.CreateVenue();
        var existing = TestData.ReserveBooking(venue, RequestedSlot);

        VenueAvailabilityService.IsAvailable(venue.Id, RequestedSlot, [existing]).ShouldBeFalse();
    }

    [Fact]
    public void GivenAConfirmedBookingAtTheSameTime_WhenCheckingAvailability_ShouldNotBeAvailable()
    {
        var venue = TestData.CreateVenue();
        var existing = TestData.ReserveBooking(venue, RequestedSlot);
        existing.Pay(new Payment(existing.Fee, PaymentMethod.Swish, Now), Now);

        VenueAvailabilityService.IsAvailable(venue.Id, RequestedSlot, [existing]).ShouldBeFalse();
    }

    [Fact]
    public void GivenACancelledBookingAtTheSameTime_WhenCheckingAvailability_ShouldBeAvailable()
    {
        var venue = TestData.CreateVenue();
        var existing = TestData.ReserveBooking(venue, RequestedSlot);
        existing.Cancel(Now);

        VenueAvailabilityService.IsAvailable(venue.Id, RequestedSlot, [existing]).ShouldBeTrue();
    }

    [Fact]
    public void GivenAnExpiredBookingAtTheSameTime_WhenCheckingAvailability_ShouldBeAvailable()
    {
        var venue = TestData.CreateVenue();
        var existing = TestData.ReserveBooking(venue, RequestedSlot);
        existing.Expire(Now.AddMinutes(16));

        VenueAvailabilityService.IsAvailable(venue.Id, RequestedSlot, [existing]).ShouldBeTrue();
    }

    [Fact]
    public void GivenABookingThatEndsWhenTheRequestedSlotStarts_WhenCheckingAvailability_ShouldBeAvailable()
    {
        var venue = TestData.CreateVenue();
        var existing = TestData.ReserveBooking(venue, SlotBetween(9, 10));

        VenueAvailabilityService.IsAvailable(venue.Id, RequestedSlot, [existing]).ShouldBeTrue();
    }

    [Fact]
    public void GivenABookingThatPartlyOverlaps_WhenCheckingAvailability_ShouldNotBeAvailable()
    {
        var venue = TestData.CreateVenue();
        var existing = TestData.ReserveBooking(venue, SlotBetween(9, 11));

        VenueAvailabilityService.IsAvailable(venue.Id, RequestedSlot, [existing]).ShouldBeFalse();
    }

    [Fact]
    public void GivenABookingAtTheSameTimeInAnotherVenue_WhenCheckingAvailability_ShouldBeAvailable()
    {
        var venue = TestData.CreateVenue();
        var otherVenue = TestData.CreateVenue();
        var existing = TestData.ReserveBooking(otherVenue, RequestedSlot);

        VenueAvailabilityService.IsAvailable(venue.Id, RequestedSlot, [existing]).ShouldBeTrue();
    }
}
