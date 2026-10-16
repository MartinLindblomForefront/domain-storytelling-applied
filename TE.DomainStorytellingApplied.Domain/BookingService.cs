namespace TE.DomainStorytellingApplied.Domain;

public class BookingService
{
    private readonly List<Booking> _bookings = [];

    public Booking Book(
        Timeslot timeslot,
        Booker? booker,
        Premises premises,
        int numberOfAttendees,
        IEnumerable<string> requiredEquipment)
    {
        if (booker is null)
            throw new InvalidOperationException("The booker does not exist");

        if (numberOfAttendees > premises.Capacity.Value)
            throw new InvalidOperationException("The premises does not have enough capacity");

        if (requiredEquipment.Any(item => !premises.Equipment.Contains(item)))
            throw new InvalidOperationException("The premises does not have the required equipment");

        if (_bookings.Any(booking => booking.TimeslotId == timeslot.Id))
            throw new InvalidOperationException("The timeslot is already booked");

        var booking = Booking.Book(timeslot.Id, booker.BookerId);

        _bookings.Add(booking);
        return booking;
    }
}