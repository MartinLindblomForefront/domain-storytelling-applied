using TE.DomainStorytellingApplied.Domain;

namespace TE.DomainStorytellingApplied.Domain.Tests;

public class BookingTests
{
    [Fact]
    public void GivenValidIds_WhenBooking_ShouldKeepAllIds()
    {
        var timeslotId = Id.Generate();
        var bookerId = Id.Generate();

        var booking = Booking.Book(timeslotId, bookerId);

        booking.BookingId.ShouldNotBeNull();
        booking.TimeslotId.ShouldBe(timeslotId);
        booking.BookerId.ShouldBe(bookerId);
    }

    [Fact]
    public void GivenAvailableTimeslot_WhenBookingThroughService_ShouldCreateBooking()
    {
        var service = new BookingService();
        var premises = CreatePremises();
        var timeslot = CreateTimeslot(premises.PremisesId);
        var booker = new Booker(Id.Generate(), BookerName.Create("Anna Andersson"));

        var booking = service.Book(timeslot, booker, premises, 8, ["Projector"]);

        booking.TimeslotId.ShouldBe(timeslot.Id);
        booking.BookerId.ShouldBe(booker.BookerId);
    }

    [Fact]
    public void GivenBookedTimeslot_WhenBookingAgain_ShouldThrow()
    {
        var service = new BookingService();
        var premises = CreatePremises();
        var timeslot = CreateTimeslot(premises.PremisesId);
        var booker = new Booker(Id.Generate(), BookerName.Create("Anna Andersson"));

        service.Book(timeslot, booker, premises, 8, []);

        Should.Throw<InvalidOperationException>(() =>
            service.Book(timeslot, booker, premises, 8, []));
    }

    [Fact]
    public void GivenMissingBooker_WhenBooking_ShouldThrow()
    {
        var premises = CreatePremises();

        Should.Throw<InvalidOperationException>(() =>
            new BookingService().Book(CreateTimeslot(premises.PremisesId), null, premises, 8, []));
    }

    [Fact]
    public void GivenTooManyAttendees_WhenBooking_ShouldThrow()
    {
        var premises = CreatePremises();
        var booker = new Booker(Id.Generate(), BookerName.Create("Anna Andersson"));

        Should.Throw<InvalidOperationException>(() =>
            new BookingService().Book(CreateTimeslot(premises.PremisesId), booker, premises, 13, []));
    }

    [Fact]
    public void GivenMissingEquipment_WhenBooking_ShouldThrow()
    {
        var premises = CreatePremises();
        var booker = new Booker(Id.Generate(), BookerName.Create("Anna Andersson"));

        Should.Throw<InvalidOperationException>(() =>
            new BookingService().Book(CreateTimeslot(premises.PremisesId), booker, premises, 8, ["Video conference"]));
    }

    private static Premises CreatePremises() =>
        new(
            Id.Generate(),
            PremisesName.Create("Conference Room"),
            Address.Create("Main Street 1"),
            Capacity.Create(12),
            ["Projector", "Whiteboard"]);

    private static Timeslot CreateTimeslot(Id premisesId) =>
        new(Id.Generate(), premisesId, DateTimeOffset.UtcNow, DateTimeOffset.UtcNow.AddHours(1));
}