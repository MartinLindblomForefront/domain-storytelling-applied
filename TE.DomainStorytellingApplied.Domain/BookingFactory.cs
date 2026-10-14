namespace TE.DomainStorytellingApplied.Domain;

public static class BookingFactory
{
    public static Booking Create(Booker booker, Premises premises, Timeslot timeslot)
    {
        var booking = new Booking(Id.Generate(), premises.Id, booker, timeslot);

        // The municipality books its own premises for free, so no payment step is needed.
        if (booker.Type == BookerType.Municipality)
            booking.Confirm();

        return booking;
    }
}
