using TE.DomainStorytellingApplied.Domain.Tests.Fakes;

namespace TE.DomainStorytellingApplied.Domain.Tests;

internal static class TestData
{
    public static readonly DateTimeOffset Now = new(2030, 1, 1, 12, 0, 0, TimeSpan.Zero);

    public static readonly Id ApproverId = new("a1");

    public static Booker Booker(string id = "b1") => new(new Id(id), "Anna");

    public static Approver Approver(string id = "a1") => new(new Id(id), "Alice");

    public static Timeslot Timeslot(string id = "t1", string premisesId = "p1", int hoursFromNow = 1, int durationHours = 1)
    {
        var start = Now.AddHours(hoursFromNow);
        return new Timeslot(new Id(id), new Id(premisesId), start, start.AddHours(durationHours));
    }

    public static Premises Premises(string id = "p1") => new(new Id(id), "Hall");

    public static Timeslot StoredTimeslot(InMemoryTimeslotRepository timeslots, string id = "t1", int hoursFromNow = 1)
    {
        var timeslot = Timeslot(id, hoursFromNow: hoursFromNow);
        timeslots.Add(timeslot);
        return timeslot;
    }

    public static Reservation PendingReservation(Booker? booker = null, Timeslot? timeslot = null)
    {
        timeslot ??= Timeslot();
        return new Reservation(new Id("r1"), (booker ?? Booker()).Id, timeslot.PremisesId, timeslot.Id);
    }

    public static BookingPolicy Policy(
        InMemoryReservationRepository? reservations = null,
        InMemoryTimeslotRepository? timeslots = null,
        int maxActiveReservations = 2,
        DateTimeOffset? now = null) =>
        new(reservations ?? new InMemoryReservationRepository(),
            timeslots ?? new InMemoryTimeslotRepository(),
            new FixedTimeProvider(now ?? Now),
            new MaxActiveReservations(maxActiveReservations));
}
