# Example: from a paid booking to a confirmation email

This is an illustration, not code that is compiled or run. It shows how the `BookingConfirmed`
domain event would travel from the booking to an email in a real system.

Only the first step is real code in this repository: `Booking.Pay` records the event, and
`BookingTests` checks that it does. Everything after that needs an Application layer and an
Infrastructure layer, so it lives here as a document.

## The flow

| Step | Who | What happens | Layer | In this repository |
|---|---|---|---|---|
| 1 | `Booking.Pay` | Confirms the booking and records `BookingConfirmed` | Domain | Yes |
| 2 | `PayBookingCommandHandler` | Saves the booking | Application | No |
| 3 | `PayBookingCommandHandler` | Passes each recorded event on | Application | No |
| 4 | `SendConfirmationEmailHandler` | Builds the email with a receipt and sends it | Application | No |
| 5 | `IEmailSender` | Talks to the mail server | Infrastructure | No |

The booking never knows that an email exists. It says "I was confirmed" and nothing more.

## Steps 2 and 3: the handler that pays

```csharp
public sealed class PayBookingCommandHandler
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IDomainEventDispatcher _domainEventDispatcher;
    private readonly TimeProvider _timeProvider;

    // Constructor left out. It only stores the three parameters.

    public async Task Handle(PayBookingCommand command, CancellationToken cancellationToken)
    {
        var booking = await _bookingRepository.GetByIdAsync(command.BookingId, cancellationToken);
        var now = _timeProvider.GetUtcNow().UtcDateTime;
        var payment = new Payment(command.Amount, command.Method, now);

        // 1. The domain checks the rules, changes the status and records BookingConfirmed
        booking.Pay(payment, now);

        // 2. Save first
        await _bookingRepository.SaveAsync(booking, cancellationToken);

        // 3. Only then tell the rest of the system
        await _domainEventDispatcher.DispatchAsync(booking.DomainEvents, cancellationToken);
    }
}
```

The handler does not mention email at all. It passes on whatever the booking recorded, and
does not know who is listening.

## Step 4: the handler that sends the email

```csharp
public sealed class SendConfirmationEmailHandler
{
    private readonly IReceiptBuilder _receiptBuilder;
    private readonly IEmailSender _emailSender;

    // Constructor left out. It only stores the two parameters.

    public async Task Handle(BookingConfirmed confirmed, CancellationToken cancellationToken)
    {
        var receipt = _receiptBuilder.Build(confirmed.BookingId, confirmed.Fee);

        await _emailSender.SendAsync(
            confirmed.ContactEmail,
            "Your booking is confirmed",
            receipt,
            cancellationToken);
    }
}
```

The event carries the three things this handler needs: which booking, who to write to and
what was paid. It does not have to load the booking again.

How the receipt looks, whether it is a PDF, and which mail server is used are not business
rules. That is why none of it is in `Booking`.

## What exists in this repository and what does not

| Name in the example | Exists here | Where it would live |
|---|---|---|
| `Booking.Pay`, `Booking.DomainEvents` | Yes | Domain |
| `BookingConfirmed`, `IDomainEvent` | Yes | Domain |
| `Payment`, `Money` | Yes | Domain |
| `PayBookingCommand`, `PayBookingCommandHandler` | No | Application |
| `SendConfirmationEmailHandler` | No | Application |
| `IDomainEventDispatcher`, `IReceiptBuilder`, `IEmailSender` | No | The interface in Application, the working class in Infrastructure |
| `IBookingRepository` | No | The interface in Application, the working class in Infrastructure |

## Why the order is save first, then dispatch

If the email went out before the save and the save then failed, the customer would hold a
confirmation for a booking that does not exist. Saving first means nobody reacts to
something that did not happen.

## Why an event, and not a call to the email sender inside `Pay`

- **The domain stays free of the outside world.** `Booking` needs no email sender, so every
  `Pay` test runs without a fake one.
- **New reactions need no change to `Booking`.** An SMS, or a line in the accounting system,
  is one more handler that listens for `BookingConfirmed`. `Pay` is not touched.
- **Each part can be tested alone.** The domain test checks that the event was recorded. A
  handler test would check that an email is sent when the event arrives.

## How the email handler would be tested

With a fake email sender that remembers what it was asked to send:

```csharp
[Fact]
public async Task GivenABookingConfirmedEvent_WhenHandling_ShouldSendAnEmailToTheContact()
{
    var emailSender = new FakeEmailSender();
    var handler = new SendConfirmationEmailHandler(new FakeReceiptBuilder(), emailSender);
    var confirmed = new BookingConfirmed(Guid.NewGuid(), "anna@example.com", new Money(400m, "SEK"));

    await handler.Handle(confirmed, CancellationToken.None);

    emailSender.SentTo.ShouldBe("anna@example.com");
}
```

No booking, no venue and no database are needed. The event is the whole input.

## Three limits of the example

- **The events are never cleared.** After dispatching, a real handler would empty the list
  (OrderSystem calls this `ClearDomainEvents`), so the same event is not sent twice. `Booking`
  has no such method yet, because nothing in this repository dispatches.
- **An event can be lost.** If the program stops between the save and the dispatch, the
  booking is confirmed and no email is sent. The fix is called the outbox pattern: the event
  is saved in the same transaction as the booking and sent afterwards.
- **A failing email must not undo the payment.** If the mail server is down, the booking is
  still paid. A real system would retry the email, not roll back the booking.
