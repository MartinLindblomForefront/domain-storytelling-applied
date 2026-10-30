namespace TE.DomainStorytellingApplied.Domain.Tests;

/// <summary>
/// Ready-made domain objects that several test classes need.
/// </summary>
public static class TestData
{
    // The moment the tests count as now, eight days before the slots they book.
    public static readonly DateTime Now = new(2026, 10, 12, 9, 0, 0, DateTimeKind.Utc);

    public static readonly ContactDetails Contact = new("Anna Andersson", "anna@example.com", "070-000 00 00");

    public static readonly OpeningHours OpeningHours = new(new TimeOnly(8, 0), new TimeOnly(22, 0));

    // Per hour: 400 SEK for private persons, 200 SEK for associations, 100 SEK for regions.
    public static readonly PriceList Prices = new(
        new Money(400m, "SEK"),
        new Money(200m, "SEK"),
        new Money(100m, "SEK"));

    // Sporthallen Norr: open 08:00 to 22:00, room for 30 people.
    // Private persons and associations may book. Regions may not.
    // Every call gives a new venue with its own Id.
    public static Venue CreateVenue(string name = "Sporthallen Norr", int capacity = 30)
    {
        return new Venue(name, capacity, OpeningHours, Prices, [BookerType.Private, BookerType.Association]);
    }

    // An unpaid booking of the venue by a private person, for 10 people, reserved at Now.
    public static Booking ReserveBooking(Venue venue, TimeSlot timeSlot)
    {
        return Booking.Reserve(venue, BookerType.Private, timeSlot, 10, Contact, Now);
    }
}
