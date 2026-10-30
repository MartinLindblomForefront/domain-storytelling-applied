namespace TE.DomainStorytellingApplied.Domain;

public enum BookingStatus
{
    Reserved,
    Confirmed
}

public class Booking
{
    public static readonly TimeSpan ReservationTimeout = TimeSpan.FromMinutes(15);

    public BookingId Id { get; }
    public TimeSlot Slot { get; }
    public BookerType BookerType { get; }
    public Money Price { get; }
    public DateTime ReservedUntil { get; }
    public BookingStatus Status { get; private set; }

    internal Booking(TimeSlot slot, BookerType bookerType, Money price, DateTime now)
    {
        Id = BookingId.New();
        Slot = slot;
        BookerType = bookerType;
        Price = price;
        Status = BookingStatus.Reserved;
        ReservedUntil = now + ReservationTimeout;
    }

    internal bool BlocksSlot(DateTime now)
    {
        if (Status == BookingStatus.Confirmed)
        {
            return true;
        }

        return now < ReservedUntil;
    }

    internal void Confirm()
    {
        Status = BookingStatus.Confirmed;
    }
}
