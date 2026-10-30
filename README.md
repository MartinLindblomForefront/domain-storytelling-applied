# Requirements
```sh
dotnet --version 
```
Should be 10 or higher. If not install the dotnet 10 SDK.

# Getting the code
```sh
git clone https://github.com/martinforefront/domain-storytelling-applied.git
```

# Building
```sh
cd domain-storytelling-applied
dotnet build
```

# Running tests
```sh
cd domain-storytelling-applied
dotnet test
```

# Structure
Domain objects goes in `TE.DomainStorytellingApplied.Domain` \
Test cases goes in `TE.DomainStorytellingApplied.Domain.Tests`

# Room booking aggregate

The solution contains a class library and tests only; there is no application.

- `RoomSchedule` is the aggregate root for one room, identified by the room ID.
  Its private list owns the room's bookings. `Reserve` checks availability,
  creates a reserved booking, and stores it. `RecordPaymentResult` confirms a
  reservation only after an approved payment attempt. `Cancel` releases its time.
  Cancelled bookings cannot be confirmed.
- `Booking` is an entity inside the schedule. Its constructor and operations used
  by the schedule are internal, so callers in other projects use the aggregate root.
  Internal access still permits other classes in the same assembly to call them.
  Confirmation itself is private and only called when recording an approved payment.
- `Room` and `Booker` are independent entities referenced by bookings.
- `TimePeriod` is a value object that requires the end to be after the start.
  Its `Overlaps` method compares periods; touching endpoints do not overlap.

Required reservation arguments (`booker`, `room`, and `period`) are checked before
creating schedules, allocating IDs, expiring reservations, or storing bookings.
Null arguments are rejected with `ArgumentNullException`. Room and booker names,
booker email, and notification recipient/message must not be null, empty, or
whitespace. Email format validation is not performed. Required constructor
dependencies are also checked immediately.

Overlapping reserved or confirmed bookings are rejected with `RoomUnavailableException`.
Adjacent periods are allowed. A booking ID cannot be reused within a schedule,
including after cancellation. Cancellation of an unknown booking is rejected.
`Booking.Status` distinguishes `Reserved`, `Confirmed`, `Cancelled`, and `Expired`.
New bookings start as `Reserved`. Only an approved result for an existing payment
attempt can confirm a booking. Repeating that result has no further effect.
`IsConfirmed` is derived from the status and is false after cancellation.

# Reservation coordination

`BookingSystem` replaces the old coordination flow and holds one `RoomSchedule`
per room ID. `BookingService` has been removed; overlap rules now live in the
aggregate root.

```csharp
// Supply an IPaymentGateway implementation (tests use StubPaymentGateway).
IPaymentGateway payments = GetPaymentGateway();
var system = new BookingSystem(payments, new NotificationService(Console.Out));
var booker = new Booker(1, "Anna", "anna@example.com");
var room = new Room(1, "Meeting room");
var period = new TimePeriod(
    new DateTime(2026, 11, 1, 10, 0, 0),
    new DateTime(2026, 11, 1, 12, 0, 0));
var result = system.Reserve(booker, room, period);
if (result.Booking is { } booking)
{
    // The booking is Reserved, not Confirmed.
    system.Cancel(room.Id, booking.Id);
}
else
{
    // result.Outcome == ReservationOutcome.RoomUnavailable
    // Show available times so the user can choose again.
}
```

`Reserve` obtains the room's schedule, allocates an ID, and delegates to
`RoomSchedule.Reserve`. It returns a `ReservationResult`: `Reserved` with a booking,
or `RoomUnavailable` with no booking. A successful reservation is stored and blocks
its time immediately, but is not confirmed. A rejected attempt creates no booking
and occupies no additional time. The result describes the reservation attempt;
the booking's current lifecycle is available through `Booking.Status`.

Reservation itself sends no message. `BookingSystem.Pay(roomId, bookingId)` starts a
payment attempt through the aggregate and passes its ID to `IPaymentGateway`.
An approved payment confirms the booking and sends a confirmation through
`NotificationService` (a TextWriter, not real email). A declined payment leaves the
booking reserved, so the caller can pay again or cancel to release the time.
Reservations allow 10 minutes to start payment; see the expiry rules below.

Use the same system instance for related booking attempts. The schedules and IDs
are in memory only. Separate instances do not share reservations. Concurrent calls
within one shared system instance are supported. A persistent, multi-process version
still needs database transactions and concurrency control across instances.

`RoomScheduleTests` covers overlap, adjacent times, duplicate IDs, and cancellation.
`BookingSystemTests` covers reservation results, room identity, cancellation,
and rejected attempts without booking objects.
Run the scenarios with `dotnet test`.

# Payment attempts

`PaymentAttempt` is an entity owned by `Booking`, with a unique Guid and a status:
`Pending`, `Succeeded`, or `Failed`. The booking exposes a read-only history.
Only the aggregate root starts attempts and records results. A reservation can
have at most one pending attempt. A confirmed or cancelled booking cannot start
a payment. Each retry after a decline creates a new attempt ID.

`IPaymentGateway.Pay(attemptId)` returns true for approval and false for a definite
decline. Tests use `StubPaymentGateway` with a sequence of approved/declined results.
There is no real payment integration, price, currency, or card handling in this example.
`GetPaymentGateway()` in the example is a placeholder for supplying that implementation.

A gateway exception leaves the attempt pending because the payment outcome is
unknown. New attempts and cancellation are blocked until the result is resolved
with `BookingSystem.RecordPaymentResult(roomId, bookingId, attemptId, succeeded)`.
This method expects a trusted payment result; no external callback endpoint is provided.
Repeated identical results have no further effect or notification; contradictory
results are rejected. The attempt ID must belong to the specified booking.

Notification failure is recorded separately and does not throw from `Pay` or
`RecordPaymentResult`. `Pay` still returns the successful payment attempt.
Automatic notification retries, refunds, and persistence are not implemented.
Cancellation of a confirmed booking still only releases its time;
it does not refund a payment. There is no direct confirmation operation; both the
aggregate and the BookingSystem flow confirm only after payment approval.
Free or manual bookings are not currently supported.

# Retry, cancel, and confirmation

All three flows use the same `BookingSystem` instance and the original booking ID:

| Flow | Calls | Final state | Confirmation |
| --- | --- | --- | --- |
| Approved immediately | `Reserve`, `Pay` | Confirmed | Once |
| Declined, then approved | `Reserve`, `Pay`, `Pay` again | Confirmed, with two attempts | Once, after approval |
| Declined, then cancelled | `Reserve`, `Pay`, `Cancel` | Cancelled; time is free | None |

After `Pay` returns a failed attempt, the caller lets the user choose whether to
call `Pay` again or `Cancel`. There is no automatic payment retry and no separate payment retry
method. The repository has no user interface yet.

The payment result and confirmed booking state are updated in memory before
`NotificationService.Send` runs. No database is present. Duplicate payment results
send no further confirmation. A notification failure keeps the booking confirmed
and the time occupied; do not retry the payment to retry message delivery.
`PaymentTests` verifies these flows, including duplicate results from earlier attempts.

## Retrying confirmation delivery

Call `GetConfirmationDelivery(roomId, bookingId)` to read an immutable result with
`Status` and `Error`. Status is `NotRequested` before payment approval, `Pending`
during delivery, `Sent` on success, or `Failed` with the error message on failure.
Call `RetryConfirmation(roomId, bookingId)` to retry a failed notification without
calling the payment gateway again. It returns the delivery result; another delivery
failure remains available for a later retry. Duplicate payment callbacks do not
retry delivery automatically.

```csharp
var attempt = system.Pay(room.Id, booking.Id);
var delivery = system.GetConfirmationDelivery(room.Id, booking.Id);
// A failed notification does not mean the payment failed.
if (delivery.Status == ConfirmationDeliveryStatus.Failed)
{
    // For example, after the delivery service has recovered:
    delivery = system.RetryConfirmation(room.Id, booking.Id);
}
```

Retries are serialized per confirmation. Once successfully sent, further retries
are no-ops. Retrying before confirmation is requested is rejected. A confirmation
not yet sent is suppressed (`Suppressed`) if the booking has been cancelled when
delivery is attempted. Cancellation cannot recall a send already in progress.
Delivery does not hold the room schedule lock.

All delivery state is in memory and is lost on restart. `Sent` means the TextWriter
returned successfully, not that a real recipient received email. A writer can
partially write and then throw; retrying can therefore duplicate partial output.
For real delivery, use a provider idempotency key and durable pending messages to
handle ambiguous delivery and restarts. Neither is implemented in this example.
`ConfirmationDeliveryTests` covers recovery, callback failures, concurrent retries,
cancellation, and booking operations during a slow notification.


# Reservation expiry and concurrency

Each booking has an `ExpiresAt` UTC timestamp. The default reservation duration is
10 minutes, configurable through `reservationDuration` on `BookingSystem` or
`RoomSchedule`. Non-positive durations are rejected. `TimeProvider` supplies the
clock, allowing tests to advance time without waiting.

- At the deadline (`now >= ExpiresAt`), a reserved booking without a pending payment
  becomes `Expired` and stops blocking its time. Payment and confirmation are rejected.
- Starting payment before the deadline holds the time until a definitive result is
  known, even past the deadline. Cancellation and additional attempts remain blocked.
- A late approval confirms that held booking. A late decline expires it and frees
  the time. A decline before the deadline permits retries within the original window.
- Failed attempts do not extend the original deadline. Confirmed and cancelled
  bookings keep their status. Duplicate results do not revive expired/cancelled bookings.

Expiry is evaluated during reservation, payment start, cancellation,
and payment-result processing. `ExpireReservations()` can also refresh stored statuses
explicitly. There is no background timer: merely reading `Booking.Status` does not
advance it. A new reservation still frees expired holds before checking availability.
An unknown payment outcome can hold a room indefinitely until reconciled with the
provider; automatic recovery from a lost payment response is not implemented.

A lock per `RoomSchedule` keeps the availability check and reservation creation
atomic, and serializes payment state changes, cancellation, and expiry. Shared
schedule lookup uses `ConcurrentDictionary`, and booking IDs use `Interlocked`.
Payment history is exposed as read-only snapshots of the attempt collection.
Gateway calls and notification delivery occur outside the schedule lock, allowing
other reservations to proceed while a payment is slow. Notification writes are
serialized so messages cannot interleave. Gateway implementations must support
concurrent calls for different bookings.

Use one shared `BookingSystem` per in-memory application. Separate instances or
processes have separate state; this is not a database-level double-booking guarantee.
`ReservationExpiryTests` covers deadlines and late payment outcomes;
`ConcurrencyTests` covers competing reservations, unique IDs, duplicate payment
responses, and a blocked gateway with concurrent operations.
