namespace TE.DomainStorytellingApplied.Domain.Tests;

public class VenueTests
{
    private static readonly DateTime Now = TestData.Now;

    // Open 09:00 to 20:00, where TestData has 08:00 to 22:00.
    private static readonly OpeningHours ShorterHours = new(new TimeOnly(9, 0), new TimeOnly(20, 0));

    // 20:00 to 21:00: open with the hours in TestData, closed with ShorterHours.
    private static readonly TimeSlot EveningSlot = new(
        new DateTime(2026, 10, 20, 20, 0, 0, DateTimeKind.Utc),
        new DateTime(2026, 10, 20, 21, 0, 0, DateTimeKind.Utc));

    private static TimeSlot SlotStartingAtTen(int minutes)
    {
        var start = new DateTime(2026, 10, 20, 10, 0, 0, DateTimeKind.Utc);

        return new TimeSlot(start, start.AddMinutes(minutes));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void GivenEmptyName_WhenCreating_ShouldThrow(string name)
    {
        Should.Throw<ArgumentException>(() => TestData.CreateVenue(name: name));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void GivenCapacityBelowOne_WhenCreating_ShouldThrow(int capacity)
    {
        Should.Throw<ArgumentException>(() => TestData.CreateVenue(capacity: capacity));
    }

    [Fact]
    public void GivenCapacityOfOne_WhenCreating_ShouldBeAllowed()
    {
        var venue = TestData.CreateVenue(capacity: 1);

        venue.Capacity.ShouldBe(1);
    }

    [Fact]
    public void GivenANewVenue_WhenCreating_ShouldGetAnId()
    {
        var venue = TestData.CreateVenue();

        venue.Id.ShouldNotBe(Guid.Empty);
    }

    [Fact]
    public void GivenTwoVenuesWithSameValues_WhenComparing_ShouldNotBeEqual()
    {
        var first = TestData.CreateVenue();
        var second = TestData.CreateVenue();

        first.ShouldNotBe(second);
        first.Id.ShouldNotBe(second.Id);
    }

    [Fact]
    public void GivenAnAssociationAndAOneHourSlot_WhenCalculatingFee_ShouldBeTheHourlyPriceForAssociations()
    {
        var venue = TestData.CreateVenue();
        var oneHour = SlotStartingAtTen(minutes: 60);

        venue.CalculateFee(oneHour, BookerType.Association).ShouldBe(new Money(200m, "SEK"));
    }

    [Fact]
    public void GivenANinetyMinuteSlot_WhenCalculatingFee_ShouldBeOneAndAHalfTimesTheHourlyPrice()
    {
        var venue = TestData.CreateVenue();
        var ninetyMinutes = SlotStartingAtTen(minutes: 90);

        venue.CalculateFee(ninetyMinutes, BookerType.Private).ShouldBe(new Money(600m, "SEK"));
    }

    [Fact]
    public void GivenANewVenue_WhenCreating_ShouldBeActive()
    {
        var venue = TestData.CreateVenue();

        venue.IsActive.ShouldBeTrue();
    }

    [Theory]
    [InlineData(BookerType.Private)]
    [InlineData(BookerType.Association)]
    public void GivenAnAllowedBookerType_WhenAskingCanBeBookedBy_ShouldBeTrue(BookerType bookerType)
    {
        var venue = TestData.CreateVenue();

        venue.CanBeBookedBy(bookerType).ShouldBeTrue();
    }

    [Fact]
    public void GivenABookerTypeThatIsNotAllowed_WhenAskingCanBeBookedBy_ShouldBeFalse()
    {
        var venue = TestData.CreateVenue();

        venue.CanBeBookedBy(BookerType.Region).ShouldBeFalse();
    }

    [Fact]
    public void GivenADeactivatedVenue_WhenAskingCanBeBookedBy_ShouldBeFalse()
    {
        var venue = TestData.CreateVenue();

        venue.Deactivate();

        venue.IsActive.ShouldBeFalse();
        venue.CanBeBookedBy(BookerType.Private).ShouldBeFalse();
    }

    // ----- Change opening hours -----

    [Fact]
    public void GivenNoBookings_WhenChangingOpeningHours_ShouldReplaceThemAndStayTheSameVenue()
    {
        var venue = TestData.CreateVenue();
        var idBefore = venue.Id;

        venue.ChangeOpeningHours(ShorterHours, [], Now);

        venue.OpeningHours.ShouldBe(ShorterHours);
        venue.Id.ShouldBe(idBefore);
    }

    [Fact]
    public void GivenChangedOpeningHours_WhenAskingIsOpenDuring_ShouldAnswerWithTheNewHours()
    {
        var venue = TestData.CreateVenue();

        venue.ChangeOpeningHours(ShorterHours, [], Now);

        // 20:00 to 21:00 was open before the change, when the venue closed at 22:00.
        venue.IsOpenDuring(EveningSlot).ShouldBeFalse();
    }

    [Fact]
    public void GivenABookingOutsideTheNewHours_WhenChangingOpeningHours_ShouldThrowAndKeepTheOldHours()
    {
        var venue = TestData.CreateVenue();
        var eveningBooking = TestData.ReserveBooking(venue, EveningSlot);

        Should.Throw<InvalidOperationException>(() => venue.ChangeOpeningHours(ShorterHours, [eveningBooking], Now));

        venue.OpeningHours.ShouldBe(TestData.OpeningHours);
    }

    [Fact]
    public void GivenABookingWithinTheNewHours_WhenChangingOpeningHours_ShouldBeAllowed()
    {
        var venue = TestData.CreateVenue();
        var middayBooking = TestData.ReserveBooking(venue, SlotStartingAtTen(minutes: 60));

        venue.ChangeOpeningHours(ShorterHours, [middayBooking], Now);

        venue.OpeningHours.ShouldBe(ShorterHours);
    }

    [Fact]
    public void GivenACancelledBookingOutsideTheNewHours_WhenChangingOpeningHours_ShouldBeAllowed()
    {
        var venue = TestData.CreateVenue();
        var eveningBooking = TestData.ReserveBooking(venue, EveningSlot);
        eveningBooking.Cancel(Now);

        venue.ChangeOpeningHours(ShorterHours, [eveningBooking], Now);

        venue.OpeningHours.ShouldBe(ShorterHours);
    }

    [Fact]
    public void GivenABookingOutsideTheNewHoursThatIsOver_WhenChangingOpeningHours_ShouldBeAllowed()
    {
        var venue = TestData.CreateVenue();
        var eveningBooking = TestData.ReserveBooking(venue, EveningSlot);
        var theDayAfter = EveningSlot.End.AddDays(1);

        venue.ChangeOpeningHours(ShorterHours, [eveningBooking], theDayAfter);

        venue.OpeningHours.ShouldBe(ShorterHours);
    }

    [Fact]
    public void GivenABookingOutsideTheNewHoursInAnotherVenue_WhenChangingOpeningHours_ShouldBeAllowed()
    {
        var venue = TestData.CreateVenue();
        var otherVenue = TestData.CreateVenue();
        var eveningBooking = TestData.ReserveBooking(otherVenue, EveningSlot);

        venue.ChangeOpeningHours(ShorterHours, [eveningBooking], Now);

        venue.OpeningHours.ShouldBe(ShorterHours);
    }

    [Fact]
    public void GivenADeactivatedVenue_WhenChangingOpeningHours_ShouldThrow()
    {
        var venue = TestData.CreateVenue();
        venue.Deactivate();

        Should.Throw<InvalidOperationException>(() => venue.ChangeOpeningHours(ShorterHours, [], Now));
    }
}
