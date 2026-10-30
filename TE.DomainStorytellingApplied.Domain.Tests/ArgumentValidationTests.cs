using TE.DomainStorytellingApplied.Domain;

namespace TE.DomainStorytellingApplied.Domain.Tests;

public class ArgumentValidationTests
{
    private readonly Booker booker = new(1, "Anna", "anna@example.com");
    private readonly Room room = new(1, "Meeting room");
    private readonly TimePeriod period = new(
        new DateTime(2026, 11, 1, 10, 0, 0), new DateTime(2026, 11, 1, 11, 0, 0));

    [Theory]
    [InlineData("booker")]
    [InlineData("room")]
    [InlineData("period")]
    public void GivenNullReservationArgument_WhenReserving_ShouldRejectWithoutCreatingSchedule(string parameter)
    {
        var payments = new StubPaymentGateway(true);
        var system = new BookingSystem(payments, new NotificationService(TextWriter.Null));

        var exception = Should.Throw<ArgumentNullException>(() => system.Reserve(
            parameter == "booker" ? null! : booker,
            parameter == "room" ? null! : room,
            parameter == "period" ? null! : period));

        exception.ParamName.ShouldBe(parameter);
        Should.Throw<KeyNotFoundException>(() => system.Cancel(room.Id, 1));
        payments.Attempts.ShouldBeEmpty();
        // The rejected request must neither consume an ID nor pin room metadata.
        var replacementRoom = new Room(room.Id, "Updated room");
        var booking = system.Reserve(booker, replacementRoom, period).Booking.ShouldNotBeNull();
        booking.Id.ShouldBe(1);
        booking.Room.ShouldBeSameAs(replacementRoom);
        system.Pay(room.Id, booking.Id).Status.ShouldBe(PaymentStatus.Succeeded);
        booking.Status.ShouldBe(BookingStatus.Confirmed);
    }

    [Theory]
    [InlineData("booker")]
    [InlineData("period")]
    public void GivenNullScheduleArgument_WhenReserving_ShouldRejectWithoutStoringBooking(string parameter)
    {
        var schedule = new RoomSchedule(room);

        var exception = Should.Throw<ArgumentNullException>(() => schedule.Reserve(1,
            parameter == "booker" ? null! : booker,
            parameter == "period" ? null! : period));

        exception.ParamName.ShouldBe(parameter);
        var booking = schedule.Reserve(1, booker, period);
        var attempt = schedule.StartPayment(booking.Id);
        schedule.RecordPaymentResult(booking.Id, attempt.Id, true).ShouldBeTrue();
        booking.Status.ShouldBe(BookingStatus.Confirmed);
    }

    [Theory]
    [InlineData("booker")]
    [InlineData("period")]
    public void GivenExpiredDeadline_WhenInvalidReservationArrives_ShouldNotMutateExistingBooking(string parameter)
    {
        var clock = new ManualTimeProvider();
        var schedule = new RoomSchedule(room, clock);
        var booking = schedule.Reserve(1, booker, period);
        clock.Advance(TimeSpan.FromMinutes(10));

        Should.Throw<ArgumentNullException>(() => schedule.Reserve(2,
            parameter == "booker" ? null! : booker,
            parameter == "period" ? null! : period));

        booking.Status.ShouldBe(BookingStatus.Reserved);
        schedule.Reserve(2, booker, period).Status.ShouldBe(BookingStatus.Reserved);
        booking.Status.ShouldBe(BookingStatus.Expired);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" \t\r\n")]
    public void GivenMissingRequiredText_WhenConstructing_ShouldReject(string? value)
    {
        Should.Throw<ArgumentException>(() => new Room(1, value!)).ParamName.ShouldBe("name");
        Should.Throw<ArgumentException>(() => new Booker(1, value!, "anna@example.com")).ParamName.ShouldBe("name");
        Should.Throw<ArgumentException>(() => new Booker(1, "Anna", value!)).ParamName.ShouldBe("email");
    }

    [Fact]
    public void GivenNullDependency_WhenConstructing_ShouldRejectImmediately()
    {
        Should.Throw<ArgumentNullException>(() => new RoomSchedule(null!)).ParamName.ShouldBe("room");
        Should.Throw<ArgumentNullException>(() => new BookingSystem(null!, new NotificationService(TextWriter.Null)))
            .ParamName.ShouldBe("payments");
        Should.Throw<ArgumentNullException>(() => new BookingSystem(new StubPaymentGateway(), null!))
            .ParamName.ShouldBe("notifications");
        Should.Throw<ArgumentNullException>(() => new NotificationService(null!)).ParamName.ShouldBe("output");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" \t")]
    public void GivenInvalidNotification_WhenSending_ShouldNotWritePartialMessage(string? value)
    {
        using var output = new StringWriter();
        var notifications = new NotificationService(output);

        Should.Throw<ArgumentException>(() => notifications.Send(value!, "Confirmed")).ParamName.ShouldBe("recipient");
        Should.Throw<ArgumentException>(() => notifications.Send("anna@example.com", value!)).ParamName.ShouldBe("message");

        output.ToString().ShouldBeEmpty();
        notifications.Send("anna@example.com", "Confirmed");
        output.ToString().ShouldContain("Confirmed");
    }
}
