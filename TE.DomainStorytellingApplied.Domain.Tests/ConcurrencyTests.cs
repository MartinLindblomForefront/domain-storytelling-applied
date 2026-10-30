using System.Collections.Concurrent;
using TE.DomainStorytellingApplied.Domain;

namespace TE.DomainStorytellingApplied.Domain.Tests;

public class ConcurrencyTests
{
    private readonly Booker booker = new(1, "Anna", "anna@example.com");
    private readonly TimePeriod period = new(
        new DateTime(2026, 11, 1, 10, 0, 0), new DateTime(2026, 11, 1, 12, 0, 0));

    [Fact]
    public void GivenConcurrentReservationsForSameRoom_WhenReserving_ShouldAllowExactlyOne()
    {
        var system = new BookingSystem(new StubPaymentGateway(), new NotificationService(TextWriter.Null));
        var results = new ConcurrentBag<ReservationResult>();

        Parallel.For(0, 100, _ => results.Add(system.Reserve(booker, new Room(1, "Room"), period)));

        results.Count(result => result.Outcome == ReservationOutcome.Reserved).ShouldBe(1);
        results.Count(result => result.Outcome == ReservationOutcome.RoomUnavailable).ShouldBe(99);
    }

    [Fact]
    public void GivenExpiredReservation_WhenConcurrentCallersReserve_ShouldAllowExactlyOneReplacement()
    {
        var clock = new ManualTimeProvider();
        var system = new BookingSystem(new StubPaymentGateway(), new NotificationService(TextWriter.Null), clock);
        var room = new Room(1, "Room");
        var expired = system.Reserve(booker, room, period).Booking.ShouldNotBeNull();
        clock.Advance(TimeSpan.FromMinutes(10));
        var results = new ConcurrentBag<ReservationResult>();

        Parallel.For(0, 100, _ => results.Add(system.Reserve(booker, room, period)));

        expired.Status.ShouldBe(BookingStatus.Expired);
        results.Count(result => result.Outcome == ReservationOutcome.Reserved).ShouldBe(1);
        results.Count(result => result.Outcome == ReservationOutcome.RoomUnavailable).ShouldBe(99);
    }

    [Fact]
    public void GivenConcurrentReservationsForDifferentRooms_WhenReserving_ShouldAllocateUniqueIds()
    {
        var system = new BookingSystem(new StubPaymentGateway(), new NotificationService(TextWriter.Null));
        var bookings = new ConcurrentBag<Booking>();

        Parallel.For(0, 100, id => bookings.Add(
            system.Reserve(booker, new Room(id, "Room"), period).Booking.ShouldNotBeNull()));

        bookings.Select(booking => booking.Id).Distinct().Count().ShouldBe(100);
    }

    [Fact]
    public void GivenConcurrentResults_WhenConfirming_ShouldNotifyExactlyOnce()
    {
        var payments = new StubPaymentGateway { ThrowOnPayment = true };
        using var output = new StringWriter();
        var system = new BookingSystem(payments, new NotificationService(output));
        var booking = system.Reserve(booker, new Room(1, "Room"), period).Booking.ShouldNotBeNull();
        Should.Throw<InvalidOperationException>(() => system.Pay(1, booking.Id));
        var attempt = booking.PaymentAttempts.Single();

        Parallel.For(0, 100, _ => system.RecordPaymentResult(1, booking.Id, attempt.Id, true));

        booking.Status.ShouldBe(BookingStatus.Confirmed);
        output.ToString().ShouldBe(
            $"Till: anna@example.com{Environment.NewLine}" +
            $"Din bokning av Room är bekräftad: 2026-11-01 10:00–2026-11-01 12:00.{Environment.NewLine}");
    }

    [Fact]
    public async Task GivenSlowPayment_WhenOtherCallsArrive_ShouldPreventDoublePaymentWithoutHoldingScheduleLock()
    {
        using var payments = new BlockingPaymentGateway();
        var system = new BookingSystem(payments, new NotificationService(TextWriter.Null));
        var room = new Room(1, "Room");
        var booking = system.Reserve(booker, room, period).Booking.ShouldNotBeNull();
        var payTask = Task.Run(() => system.Pay(room.Id, booking.Id));
        try
        {
            payments.Started.Wait(TimeSpan.FromSeconds(10), TestContext.Current.CancellationToken).ShouldBeTrue();
            await Task.Run(() =>
            {
                Should.Throw<InvalidOperationException>(() => system.Pay(room.Id, booking.Id));
                Should.Throw<InvalidOperationException>(() => system.Cancel(room.Id, booking.Id));
                system.Reserve(booker, room, period).Outcome.ShouldBe(ReservationOutcome.RoomUnavailable);
                var adjacent = new TimePeriod(period.End, period.End.AddHours(1));
                system.Reserve(booker, room, adjacent).Outcome.ShouldBe(ReservationOutcome.Reserved);
            }, TestContext.Current.CancellationToken).WaitAsync(TimeSpan.FromSeconds(5), TestContext.Current.CancellationToken);
        }
        finally
        {
            payments.Release.Set();
            await payTask.WaitAsync(TimeSpan.FromSeconds(10), CancellationToken.None);
        }

        payments.Calls.ShouldBe(1);
        booking.Status.ShouldBe(BookingStatus.Confirmed);
    }

    private sealed class BlockingPaymentGateway : IPaymentGateway, IDisposable
    {
        public ManualResetEventSlim Started { get; } = new();
        public ManualResetEventSlim Release { get; } = new();
        private int calls;
        public int Calls => Volatile.Read(ref calls);

        public bool Pay(Guid paymentAttemptId)
        {
            Interlocked.Increment(ref calls);
            Started.Set();
            if (!Release.Wait(TimeSpan.FromSeconds(15))) throw new TimeoutException();
            return true;
        }

        public void Dispose()
        {
            Started.Dispose();
            Release.Dispose();
        }
    }
}
