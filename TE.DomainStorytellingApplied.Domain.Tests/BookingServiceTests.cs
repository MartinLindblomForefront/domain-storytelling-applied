namespace TE.DomainStorytellingApplied.Domain.Tests;

public class BookingServiceTests
{
    private static readonly DateTimeOffset Morning = new(2026, 10, 9, 8, 0, 0, TimeSpan.Zero);
    private static readonly Timeslot Slot = new(Morning, Morning.AddHours(1));

    private static readonly Booker PrivateBooker = new("Anna", BookerType.Private);
    private static readonly Booker Municipality = new("Kommunen", BookerType.Municipality);

    private readonly InMemoryBookingRepository _repository = new();
    private readonly FakePaymentGateway _paymentGateway = new();
    private readonly Premises _premises = new(Id.Generate(), "Fotbollsplan", 300m, [Slot]);

    private BookingService CreateService() => new(_repository, _paymentGateway);

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

    [Fact]
    public void GivenSuccessfulPayment_WhenPayingForBooking_ShouldBeConfirmed()
    {
        var service = CreateService();
        var booking = service.RequestBooking(PrivateBooker, _premises, Slot);

        service.PayForBooking(booking.Id, _premises);

        booking.Status.ShouldBe(BookingStatus.Confirmed);
        _premises.IsReserved(Slot).ShouldBeTrue();
        _paymentGateway.LastAmount.ShouldBe(_premises.Price);
    }

    [Fact]
    public void GivenFailedPayment_WhenPayingForBooking_ShouldBeCancelledAndReleaseTimeslot()
    {
        var service = CreateService();
        var booking = service.RequestBooking(PrivateBooker, _premises, Slot);
        _paymentGateway.ResultToReturn = PaymentResult.Failure;

        service.PayForBooking(booking.Id, _premises);

        booking.Status.ShouldBe(BookingStatus.Cancelled);
        _premises.IsReserved(Slot).ShouldBeFalse();
    }

    [Fact]
    public void GivenMunicipalityBooking_WhenPayingForBooking_ShouldThrowWithoutCallingGateway()
    {
        var service = CreateService();
        var booking = service.RequestBooking(Municipality, _premises, Slot);

        Should.Throw<Exception>(() => service.PayForBooking(booking.Id, _premises));
        _paymentGateway.CallCount.ShouldBe(0);
    }

    [Fact]
    public void GivenUnknownBooking_WhenPayingForBooking_ShouldThrow()
    {
        Should.Throw<Exception>(() => CreateService().PayForBooking(Id.Generate(), _premises));
    }
}
