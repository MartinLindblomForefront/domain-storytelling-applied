using static TE.DomainStorytellingApplied.Domain.Tests.TestData;

namespace TE.DomainStorytellingApplied.Domain.Tests;

public class VenueReserveTests
{
    [Fact]
    public void GivenAssociation_WhenReserving_ShouldBeReservedAndCostHourlyRate()
    {
        var booking = NewVenue().Reserve(Tomorrow18, BookerType.Association, Now);

        booking.Status.ShouldBe(BookingStatus.Reserved);
        booking.Price.ShouldBe(new Money(100));
    }

    [Fact]
    public void GivenMunicipality_WhenReserving_ShouldBeConfirmedImmediatelyAndFree()
    {
        var booking = NewVenue().Reserve(Tomorrow18, BookerType.Municipality, Now);

        booking.Status.ShouldBe(BookingStatus.Confirmed);
        booking.Price.IsZero.ShouldBeTrue();
    }

    [Fact]
    public void GivenTimeInThePast_WhenReserving_ShouldThrow()
    {
        var pastSlot = new TimeSlot(new DateTime(2026, 10, 9, 8, 0, 0));

        Should.Throw<DomainException>(() => NewVenue().Reserve(pastSlot, BookerType.Association, Now));
    }

    [Theory]
    [InlineData(7)]
    [InlineData(22)]
    public void GivenOutsideOpeningHours_WhenReserving_ShouldThrow(int startHour)
    {
        var slot = new TimeSlot(new DateTime(2026, 10, 10, startHour, 0, 0));

        Should.Throw<DomainException>(() => NewVenue().Reserve(slot, BookerType.Association, Now));
    }

    [Fact]
    public void GivenSlotAlreadyReserved_WhenReservingAgain_ShouldThrow()
    {
        var venue = NewVenue();
        venue.Reserve(Tomorrow18, BookerType.Association, Now);

        Should.Throw<DomainException>(() => venue.Reserve(Tomorrow18, BookerType.Municipality, Now));
    }
}
