namespace TE.DomainStorytellingApplied.Domain.Tests;

public class BookingServiceTests
{
    private static readonly DateTimeOffset Morning = new(2026, 10, 9, 8, 0, 0, TimeSpan.Zero);
    private static readonly Timeslot Slot = new(Morning, Morning.AddHours(1));

    private static readonly Booker PrivateBooker = new("Anna", BookerType.Private);
    private static readonly Booker Municipality = new("Kommunen", BookerType.Municipality);

    private readonly InMemoryBookingRepository _repository = new();
    private readonly Premises _premises = new(Id.Generate(), "Fotbollsplan", 300m, [Slot]);

    private BookingService CreateService() => new(_repository);

    [Fact]
    public void GivenFreeTimeslot_WhenRequestingBooking_ShouldReserveTimeslot()
    {
        CreateService().RequestBooking(PrivateBooker, _premises, Slot);

        _premises.IsReserved(Slot).ShouldBeTrue();
    }

    [Fact]
    public void GivenFreeTimeslot_WhenRequestingBooking_ShouldStoreBooking()
    {
        var booking = CreateService().RequestBooking(PrivateBooker, _premises, Slot);

        _repository.GetById(booking.Id).ShouldBe(booking);
    }

    [Fact]
    public void GivenPrivateBooker_WhenRequestingBooking_ShouldBePending()
    {
        var booking = CreateService().RequestBooking(PrivateBooker, _premises, Slot);

        booking.Status.ShouldBe(BookingStatus.Pending);
    }

    [Fact]
    public void GivenMunicipality_WhenRequestingBooking_ShouldBeConfirmed()
    {
        var booking = CreateService().RequestBooking(Municipality, _premises, Slot);

        booking.Status.ShouldBe(BookingStatus.Confirmed);
    }

    [Fact]
    public void GivenAlreadyReservedTimeslot_WhenRequestingBooking_ShouldThrow()
    {
        var service = CreateService();
        service.RequestBooking(PrivateBooker, _premises, Slot);

        Should.Throw<Exception>(() => service.RequestBooking(Municipality, _premises, Slot));
    }
}
