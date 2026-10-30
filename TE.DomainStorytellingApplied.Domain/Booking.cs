namespace TE.DomainStorytellingApplied.Domain;

public enum BookingStatus
{
    Reserved,
    Confirmed,
    Cancelled,
    Expired
}

public class Booking
{
    private volatile PaymentAttempt[] paymentAttempts = [];

    public IReadOnlyList<PaymentAttempt> PaymentAttempts => Array.AsReadOnly(paymentAttempts);

    internal Booking(int id, Room room, TimePeriod period, Booker booker, DateTimeOffset expiresAt)
    {
        Id = id;
        Room = room;
        Period = period;
        Booker = booker;
        ExpiresAt = expiresAt;
    }

    public DateTimeOffset ExpiresAt { get; }
    public int Id { get; }
    public Room Room { get; }
    public TimePeriod Period { get; }
    public Booker Booker { get; }
    private volatile BookingStatus status = BookingStatus.Reserved;
    public BookingStatus Status => status;
    public bool IsConfirmed => Status == BookingStatus.Confirmed;

    private void Confirm()
    {
        if (Status is BookingStatus.Cancelled or BookingStatus.Expired)
        {
            throw new InvalidOperationException("En avbruten eller utgången bokning kan inte bekräftas.");
        }

        status = BookingStatus.Confirmed;
    }

    internal PaymentAttempt StartPayment()
    {
        if (Status != BookingStatus.Reserved)
        {
            throw new InvalidOperationException("Bara en reserverad bokning kan betalas.");
        }

        if (paymentAttempts.Any(attempt => attempt.Status == PaymentStatus.Pending))
        {
            throw new InvalidOperationException("Ett betalningsförsök pågår redan.");
        }

        var attempt = new PaymentAttempt();
        paymentAttempts = [..paymentAttempts, attempt];
        return attempt;
    }

    internal bool RecordPaymentResult(Guid paymentAttemptId, bool succeeded)
    {
        var attempt = paymentAttempts.SingleOrDefault(attempt => attempt.Id == paymentAttemptId)
            ?? throw new KeyNotFoundException("Betalningsförsöket finns inte på bokningen.");

        bool changed = attempt.RecordResult(succeeded);
        if (changed && succeeded)
        {
            Confirm();
            return true;
        }

        return false;
    }

    internal void Expire(DateTimeOffset now)
    {
        if (Status == BookingStatus.Reserved && now >= ExpiresAt &&
            !paymentAttempts.Any(attempt => attempt.Status == PaymentStatus.Pending))
        {
            status = BookingStatus.Expired;
        }
    }

    internal void Cancel()
    {
        if (paymentAttempts.Any(attempt => attempt.Status == PaymentStatus.Pending))
        {
            throw new InvalidOperationException("Invänta betalningsresultatet innan bokningen avbryts.");
        }

        if (Status != BookingStatus.Expired)
        {
            status = BookingStatus.Cancelled;
        }
    }
}
