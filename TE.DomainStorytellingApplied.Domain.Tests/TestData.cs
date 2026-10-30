namespace TE.DomainStorytellingApplied.Domain.Tests;

internal static class TestData
{
    public static readonly DateTime Now = new DateTime(2026, 10, 9, 8, 30, 0);

    public static readonly TimeSlot Tomorrow18 = new TimeSlot(new DateTime(2026, 10, 10, 18, 0, 0));

    public static Venue NewVenue()
    {
        return new Venue("Sporthallen", new Money(100), 8, 22);
    }

    public static DateTime AfterTimeout(DateTime reservedAt)
    {
        return reservedAt + Booking.ReservationTimeout;
    }
}
