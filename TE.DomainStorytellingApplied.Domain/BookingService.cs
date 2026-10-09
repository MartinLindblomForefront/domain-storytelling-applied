namespace TE.DomainStorytellingApplied.Domain;

public sealed class BookingService
{
    private readonly IBookingRepository _bookingRepository;

    public BookingService(IBookingRepository bookingRepository)
    {
        _bookingRepository = bookingRepository;
    }

    public Booking RequestBooking(Booker booker, Premises premises, Timeslot timeslot)
    {
        premises.Reserve(timeslot);

        var booking = BookingFactory.Create(booker, premises, timeslot);
        _bookingRepository.Add(booking);

        return booking;
    }
}
