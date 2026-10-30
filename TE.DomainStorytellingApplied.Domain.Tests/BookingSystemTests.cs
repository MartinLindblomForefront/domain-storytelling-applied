using TE.DomainStorytellingApplied.Domain;

namespace TE.DomainStorytellingApplied.Domain.Tests;

public class BookingSystemTests
{
    private readonly Booker booker = new(1, "Anna", "anna@example.com");

    [Fact]
    public void GivenAvailableRoom_WhenReserving_ShouldKeepDetailsWithoutConfirming()
    {
        var system = new BookingSystem(new StubPaymentGateway(false), new NotificationService(TextWriter.Null));
        var room = new Room(1, "Meeting room");
        var period = CreatePeriod(10, 11);

        var result = system.Reserve(booker, room, period);

        result.Outcome.ShouldBe(ReservationOutcome.Reserved);
        var booking = result.Booking.ShouldNotBeNull();
        booking.Booker.ShouldBeSameAs(booker);
        booking.Room.ShouldBeSameAs(room);
        booking.Period.ShouldBe(period);
        booking.Status.ShouldBe(BookingStatus.Reserved);
        booking.IsConfirmed.ShouldBeFalse();
    }

    [Fact]
    public void GivenReservedRoom_WhenReservingSameTime_ShouldReturnUnavailableWithoutBooking()
    {
        var system = new BookingSystem(new StubPaymentGateway(false), new NotificationService(TextWriter.Null));
        var room = new Room(1, "Meeting room");
        var period = CreatePeriod(10, 12);
        var first = system.Reserve(booker, room, period).Booking.ShouldNotBeNull();

        var second = system.Reserve(new Booker(2, "Bertil", "bertil@example.com"), room, period);

        second.Outcome.ShouldBe(ReservationOutcome.RoomUnavailable);
        second.Booking.ShouldBeNull();
        first.Status.ShouldBe(BookingStatus.Reserved);
    }

    [Fact]
    public void GivenAdjacentPeriod_WhenReserving_ShouldReserveBothWithDifferentIds()
    {
        var system = new BookingSystem(new StubPaymentGateway(false), new NotificationService(TextWriter.Null));
        var room = new Room(1, "Meeting room");

        var first = system.Reserve(booker, room, CreatePeriod(10, 11)).Booking.ShouldNotBeNull();
        var second = system.Reserve(booker, room, CreatePeriod(11, 12)).Booking.ShouldNotBeNull();

        first.Status.ShouldBe(BookingStatus.Reserved);
        second.Status.ShouldBe(BookingStatus.Reserved);
        second.Id.ShouldNotBe(first.Id);
    }

    [Fact]
    public void GivenRejectedReservation_WhenReservingItsFreePart_ShouldSucceed()
    {
        var system = new BookingSystem(new StubPaymentGateway(false), new NotificationService(TextWriter.Null));
        var room = new Room(1, "Meeting room");
        system.Reserve(booker, room, CreatePeriod(10, 12));
        var rejected = system.Reserve(booker, room, CreatePeriod(9, 11));

        var result = system.Reserve(booker, room, CreatePeriod(9, 10));

        rejected.Outcome.ShouldBe(ReservationOutcome.RoomUnavailable);
        rejected.Booking.ShouldBeNull();
        result.Outcome.ShouldBe(ReservationOutcome.Reserved);
        result.Booking.ShouldNotBeNull().Status.ShouldBe(BookingStatus.Reserved);
    }

    [Fact]
    public void GivenDifferentRooms_WhenReservingSameTime_ShouldReserveBoth()
    {
        var system = new BookingSystem(new StubPaymentGateway(false), new NotificationService(TextWriter.Null));

        var first = system.Reserve(booker, new Room(1, "Meeting room"), CreatePeriod(10, 12));
        var second = system.Reserve(booker, new Room(2, "Meeting room"), CreatePeriod(10, 12));

        first.Outcome.ShouldBe(ReservationOutcome.Reserved);
        second.Outcome.ShouldBe(ReservationOutcome.Reserved);
    }

    [Fact]
    public void GivenSameRoomIdInDifferentObjects_WhenReservingSameTime_ShouldReject()
    {
        var system = new BookingSystem(new StubPaymentGateway(false), new NotificationService(TextWriter.Null));
        system.Reserve(booker, new Room(1, "Meeting room"), CreatePeriod(10, 12));

        var second = system.Reserve(booker, new Room(1, "Meeting room"), CreatePeriod(10, 12));

        second.Outcome.ShouldBe(ReservationOutcome.RoomUnavailable);
        second.Booking.ShouldBeNull();
    }

    [Fact]
    public void GivenCancelledReservation_WhenReservingAgain_ShouldReuseFreedTime()
    {
        var system = new BookingSystem(new StubPaymentGateway(false), new NotificationService(TextWriter.Null));
        var room = new Room(1, "Meeting room");
        var first = system.Reserve(booker, room, CreatePeriod(10, 12)).Booking.ShouldNotBeNull();

        system.Cancel(room.Id, first.Id);
        var second = system.Reserve(booker, room, CreatePeriod(10, 12)).Booking.ShouldNotBeNull();

        first.Status.ShouldBe(BookingStatus.Cancelled);
        second.Status.ShouldBe(BookingStatus.Reserved);
        second.Id.ShouldNotBe(first.Id);
    }

    [Fact]
    public void GivenUnknownRoom_WhenCancelling_ShouldReject()
    {
        var system = new BookingSystem(new StubPaymentGateway(false), new NotificationService(TextWriter.Null));

        Should.Throw<KeyNotFoundException>(() => system.Cancel(1, 1));
    }

    private static TimePeriod CreatePeriod(int startHour, int endHour) => new(
        new DateTime(2026, 11, 1, startHour, 0, 0),
        new DateTime(2026, 11, 1, endHour, 0, 0));
}
