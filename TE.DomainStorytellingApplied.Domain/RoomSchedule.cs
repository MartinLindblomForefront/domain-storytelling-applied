namespace TE.DomainStorytellingApplied.Domain;


public class RoomSchedule
{
    private readonly List<Booking> bookings = [];
    private readonly object gate = new();

    private readonly TimeProvider clock;
    private readonly TimeSpan reservationDuration;

    public RoomSchedule(Room room, TimeProvider? clock = null, TimeSpan? reservationDuration = null)
    {
        ArgumentNullException.ThrowIfNull(room);

        Room = room;
        this.clock = clock ?? TimeProvider.System;
        this.reservationDuration = reservationDuration ?? TimeSpan.FromMinutes(10);
        if (this.reservationDuration <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(reservationDuration));
        }
    }

    public int Id => Room.Id;
    public Room Room { get; }

    public Booking Reserve(int bookingId, Booker booker, TimePeriod period)
    {
        ArgumentNullException.ThrowIfNull(booker);
        ArgumentNullException.ThrowIfNull(period);

        lock (gate)
        {
            ExpireReservations();
            if (bookings.Any(existing => existing.Id == bookingId))
            {
                throw new ArgumentException("Boknings-id finns redan i schemat.", nameof(bookingId));
            }

            bool overlaps = bookings.Any(existing =>
                (existing.Status is BookingStatus.Reserved or BookingStatus.Confirmed) &&
                period.Overlaps(existing.Period));

            if (overlaps)
            {
                throw new RoomUnavailableException();
            }

            var booking = new Booking(bookingId, Room, period, booker, clock.GetUtcNow() + reservationDuration);

            bookings.Add(booking);
            return booking;
        }
    }

    public PaymentAttempt StartPayment(int bookingId)
    {
        lock (gate)
        {
            ExpireReservations();
            return FindBooking(bookingId).StartPayment();
        }
    }

    public bool RecordPaymentResult(int bookingId, Guid paymentAttemptId, bool succeeded)
    {
        lock (gate)
        {
            var booking = FindBooking(bookingId);
            bool newlyConfirmed = booking.RecordPaymentResult(paymentAttemptId, succeeded);
            booking.Expire(clock.GetUtcNow());
            return newlyConfirmed;
        }
    }

    internal Booking FindBooking(int bookingId)
    {
        lock (gate)
        {
            return bookings.SingleOrDefault(existing => existing.Id == bookingId)
                ?? throw new KeyNotFoundException("Bokningen finns inte i rummets schema.");
        }
    }

    public void Cancel(int bookingId)
    {
        lock (gate)
        {
            ExpireReservations();
            FindBooking(bookingId).Cancel();
        }
    }

    public void ExpireReservations()
    {
        lock (gate)
        {
            var now = clock.GetUtcNow();
            foreach (var booking in bookings)
            {
                booking.Expire(now);
            }
        }
    }
}
