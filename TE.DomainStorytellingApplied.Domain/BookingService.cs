namespace TE.DomainStorytellingApplied.Domain;

public sealed class BookingService
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IPaymentGateway _paymentGateway;

    public BookingService(IBookingRepository bookingRepository, IPaymentGateway paymentGateway)
    {
        _bookingRepository = bookingRepository;
        _paymentGateway = paymentGateway;
    }

    public Booking RequestBooking(Booker booker, Premises premises, Timeslot timeslot)
    {
        premises.Reserve(timeslot);

        var booking = BookingFactory.Create(booker, premises, timeslot);
        _bookingRepository.Add(booking);

        return booking;
    }

    public Booking PayForBooking(Id bookingId, Premises premises)
    {
        var booking = _bookingRepository.GetById(bookingId)
            ?? throw new Exception("Booking not found");

        if (booking.PremisesId != premises.Id)
            throw new Exception("Booking does not belong to this premises");

        if (booking.Status != BookingStatus.Pending)
            throw new Exception($"Only a pending booking can be paid, status is {booking.Status}");

        var result = _paymentGateway.Pay(booking.Id, premises.Price);

        if (result == PaymentResult.Success)
        {
            booking.Confirm();
        }
        else
        {
            booking.Cancel();
            premises.Release(booking.Timeslot);
        }

        return booking;
    }
}
