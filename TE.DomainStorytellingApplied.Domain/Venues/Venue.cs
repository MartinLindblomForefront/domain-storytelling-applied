namespace TE.DomainStorytellingApplied.Domain;

public class Venue
{
    private readonly List<Booking> _bookings = new List<Booking>();

    public VenueId Id { get; }
    public string Name { get; }
    public Money HourlyRate { get; }
    public int OpensHour { get; }
    public int ClosesHour { get; }

    public IReadOnlyList<Booking> Bookings
    {
        get { return _bookings; }
    }

    public Venue(string name, Money hourlyRate, int opensHour, int closesHour)
    {
        Id = VenueId.New();
        Name = name;
        HourlyRate = hourlyRate;
        OpensHour = opensHour;
        ClosesHour = closesHour;
    }

    public bool IsAvailable(TimeSlot slot, DateTime now)
    {
        foreach (var booking in _bookings)
        {
            if (booking.BlocksSlot(now) && booking.Slot == slot)
            {
                return false;
            }
        }

        return true;
    }

    public IReadOnlyList<TimeSlot> FreeHours(DateTime day, DateTime now)
    {
        var free = new List<TimeSlot>();

        for (var hour = OpensHour; hour < ClosesHour; hour++)
        {
            var slot = new TimeSlot(day.Date.AddHours(hour));

            if (slot.Start > now && IsAvailable(slot, now))
            {
                free.Add(slot);
            }
        }

        return free;
    }

    public Booking Reserve(TimeSlot slot, BookerType bookerType, DateTime now)
    {
        if (slot.Start <= now)
        {
            throw new DomainException("Tiden har redan börjat eller passerat.");
        }

        if (slot.Start.Hour < OpensHour || slot.Start.Hour >= ClosesHour)
        {
            throw new DomainException("Tiden ligger utanför lokalens öppettider.");
        }

        if (!IsAvailable(slot, now))
        {
            throw new DomainException("Tiden är redan bokad.");
        }

        var price = HourlyRate;
        if (bookerType == BookerType.Municipality)
        {
            price = Money.Zero;
        }

        var booking = new Booking(slot, bookerType, price, now);

        if (price.IsZero)
        {
            booking.Confirm();
        }

        _bookings.Add(booking);

        return booking;
    }

    public void ConfirmBooking(BookingId id, DateTime now)
    {
        var booking = Find(id);

        if (booking.Status != BookingStatus.Reserved)
        {
            throw new DomainException("Bokningen väntar inte på betalning.");
        }

        if (now >= booking.ReservedUntil)
        {
            throw new DomainException("Reservationen har gått ut.");
        }

        booking.Confirm();
    }

    private Booking Find(BookingId id)
    {
        foreach (var booking in _bookings)
        {
            if (booking.Id == id)
            {
                return booking;
            }
        }

        throw new DomainException("Bokningen finns inte.");
    }
}
