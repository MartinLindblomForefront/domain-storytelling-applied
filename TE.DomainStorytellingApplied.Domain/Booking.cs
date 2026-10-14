namespace TE.DomainStorytellingApplied.Domain;

public enum BookingStatus
{
    Pending,
    Confirmed,
    Cancelled
}

public sealed class Booking
{
    public Id Id { get; }

    public Id PremisesId { get; }

    public Booker Booker { get; }

    public Timeslot Timeslot { get; }

    public BookingStatus Status { get; private set; }

    public Booking(Id id, Id premisesId, Booker booker, Timeslot timeslot)
    {
        Id = id;
        PremisesId = premisesId;
        Booker = booker;
        Timeslot = timeslot;
        Status = BookingStatus.Pending;
    }

    public void Confirm()
    {
        if (Status != BookingStatus.Pending)
            throw new Exception($"Only a pending booking can be confirmed, status is {Status}");

        Status = BookingStatus.Confirmed;
    }

    public void Cancel()
    {
        if (Status != BookingStatus.Pending)
            throw new Exception($"Only a pending booking can be cancelled, status is {Status}");

        Status = BookingStatus.Cancelled;
    }
}
