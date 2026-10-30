# Example: a command handler that reserves a booking

This is an illustration, not code that is compiled or run. It shows how a real system would
call the domain in this repository. A handler belongs to the Application layer, and this
repository only has the Domain layer, so the example lives here as a document.

In this repository the tests play the role of the handler: they create a venue, pick a time
and call `Booking.Reserve`.

## The handler

```csharp
public sealed class ReserveBookingCommandHandler
{
    private readonly IVenueRepository _venueRepository;
    private readonly IBookingRepository _bookingRepository;
    private readonly ICurrentUser _currentUser;
    private readonly TimeProvider _timeProvider;

    public ReserveBookingCommandHandler(
        IVenueRepository venueRepository,
        IBookingRepository bookingRepository,
        ICurrentUser currentUser,
        TimeProvider timeProvider)
    {
        _venueRepository = venueRepository;
        _bookingRepository = bookingRepository;
        _currentUser = currentUser;
        _timeProvider = timeProvider;
    }

    public async Task<Guid> Handle(ReserveBookingCommand command, CancellationToken cancellationToken)
    {
        // 1. Load what the domain needs
        var venue = await _venueRepository.GetByIdAsync(command.VenueId, cancellationToken);

        // 2. Turn the raw input into value objects
        var timeSlot = new TimeSlot(command.Start, command.End);
        var contact = new ContactDetails(command.Name, command.Email, command.Phone);
        var now = _timeProvider.GetUtcNow().UtcDateTime;

        // 3. The rule that spans many bookings
        var existingBookings = await _bookingRepository.GetForVenueAsync(venue.Id, cancellationToken);

        if (!VenueAvailabilityService.IsAvailable(venue.Id, timeSlot, existingBookings))
        {
            throw new InvalidOperationException("The time is already booked.");
        }

        // 4. The rules that belong to one booking
        var booking = Booking.Reserve(venue, _currentUser.BookerType, timeSlot, command.ParticipantCount, contact, now);

        // 5. Save
        await _bookingRepository.AddAsync(booking, cancellationToken);

        return booking.Id;
    }
}
```

## What exists in this repository and what does not

| Name in the example | Exists here | Where it would live |
|---|---|---|
| `TimeSlot`, `ContactDetails` | Yes | Domain |
| `Venue`, `Booking`, `Booking.Reserve` | Yes | Domain |
| `VenueAvailabilityService.IsAvailable` | Yes | Domain |
| `ReserveBookingCommand` | No | Application |
| `ReserveBookingCommandHandler` | No | Application |
| `IVenueRepository`, `IBookingRepository` | No | The interface in Application, the class that talks to the database in Infrastructure |
| `ICurrentUser` | No | The interface in Application, the class that reads the login token in Web |
| `TimeProvider` | Built into .NET | |

## Where each parameter of `Booking.Reserve` comes from

| Parameter | Where the handler gets it |
|---|---|
| `venue` | Loaded from the database, using the venue id the user picked |
| `bookerType` | From `ICurrentUser`, which reads it from the login token. It is not on the command, so the user cannot type it in. See `ExampleCurrentUserFromJwt.md` |
| `timeSlot` | Built from the start and end the user chose |
| `participantCount` | Typed in by the user |
| `contact` | Built from the form fields |
| `now` | From a clock (`TimeProvider`), which a test can replace |

## What the handler does and does not do

The handler fetches, converts, calls and saves. It decides nothing.

Every "is this allowed?" is answered by the domain:

- `TimeSlot` and `ContactDetails` check their own input when they are created.
- `VenueAvailabilityService` checks that the venue is not already booked at that time.
- `Booking.Reserve` checks the venue's rules: who may book, opening hours and capacity.

That is why `Booking.Reserve` takes a finished `Venue` and a finished `DateTime`, and not a
venue id and a clock. If it took an id, it would need a repository to look the venue up, and
the domain would depend on a database. As it is, the handler does the fetching and the
domain does the thinking.

## How this repository compares

| Job | Real system | This repository |
|---|---|---|
| Provide the venue | A repository | `CreateVenue()` in the test |
| Provide `now` | `TimeProvider` | The `Now` constant in the test |
| Provide the booker type | `ICurrentUser` | `BookerType.Private` written in the test |
| Check availability | The handler calls `VenueAvailabilityService` | The test calls it |
| Call `Booking.Reserve` | The handler | The test |
| Save the booking | A repository | Nothing, the test just looks at it |

## Two limits of the example

- **Nothing forces step 3.** A handler that forgets to call `VenueAvailabilityService` still
  compiles. In a real system a handler test would prove the call is made.
- **Two people can reserve in the same instant.** Both can pass step 3 before either has
  saved. Only a constraint in the database closes that gap.
