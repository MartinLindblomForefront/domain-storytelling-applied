namespace TE.DomainStorytellingApplied.Domain.Tests;

public class BookingFactoryTests
{
    private static readonly DateTimeOffset Morning = new(2026, 10, 9, 8, 0, 0, TimeSpan.Zero);
    private static readonly Timeslot Slot = new(Morning, Morning.AddHours(1));

    private static Premises CreatePremises() =>
        new(Id.Generate(), "Konferenssal", 500m, [Slot]);

    [Theory]
    [InlineData(BookerType.Private)]
    [InlineData(BookerType.Association)]
    public void GivenPayingBooker_WhenCreatingBooking_ShouldBePending(BookerType type)
    {
        var booking = BookingFactory.Create(new Booker("Anna", type), CreatePremises(), Slot);

        booking.Status.ShouldBe(BookingStatus.Pending);
    }

    [Fact]
    public void GivenMunicipalityBooker_WhenCreatingBooking_ShouldBeConfirmed()
    {
        var booking = BookingFactory.Create(new Booker("Kommunen", BookerType.Municipality), CreatePremises(), Slot);

        booking.Status.ShouldBe(BookingStatus.Confirmed);
    }

    [Fact]
    public void GivenPremisesAndTimeslot_WhenCreatingBooking_ShouldReferenceThem()
    {
        var premises = CreatePremises();

        var booking = BookingFactory.Create(new Booker("Anna", BookerType.Private), premises, Slot);

        booking.PremisesId.ShouldBe(premises.Id);
        booking.Timeslot.ShouldBe(Slot);
    }
}
