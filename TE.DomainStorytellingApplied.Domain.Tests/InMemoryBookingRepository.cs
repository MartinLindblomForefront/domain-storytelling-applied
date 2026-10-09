namespace TE.DomainStorytellingApplied.Domain.Tests;

public class InMemoryBookingRepository : IBookingRepository
{
    private readonly Dictionary<Id, Booking> _bookings = [];

    public void Add(Booking booking) => _bookings.Add(booking.Id, booking);

    public Booking? GetById(Id id) => _bookings.GetValueOrDefault(id);
}
