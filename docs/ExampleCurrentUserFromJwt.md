# Example: getting the booker type from the logged-in user

This is an illustration, not code that is compiled or run. It shows how a real system would
find out what type of booker is making a request, and hand that to the domain.

`Booking.Reserve` takes a `BookerType` and trusts it. The booker type decides who may book a
venue and which price applies, so it must not be something the user can type in. In a real
system it comes from the login token (a JWT), which the server has verified.

Nothing in this repository reads a token. In the tests, `BookerType.Private` is written
straight into the call, and stands in for everything described here.

## The flow

| Step | Who | What happens | Layer | In this repository |
|---|---|---|---|---|
| 1 | The login | Writes the booker type into the token and signs it | Outside | No |
| 2 | ASP.NET Core | Verifies the token on every request | Web | No |
| 3 | `HttpContextCurrentUser` | Reads the booker type out of the token | Web | No |
| 4 | `ReserveBookingCommandHandler` | Asks `ICurrentUser` for the booker type | Application | No |
| 5 | `Booking.Reserve` | Uses it to check the venue's rules and the fee | Domain | Yes |

This document is about steps 3 and 4: one interface and the one class behind it.

## The interface

```csharp
public interface ICurrentUser
{
    BookerType BookerType { get; }
}
```

It asks one question: what type of booker is making this request? It says nothing about how
the answer is found. There is no mention of a token, a request or a database.

That is why it can live in the Application layer. The handler depends on this interface, and
never learns that a JWT exists.

## The class that reads the token

```csharp
public sealed class HttpContextCurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpContextCurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public BookerType BookerType
    {
        get
        {
            var claim = _httpContextAccessor.HttpContext?.User.FindFirst("booker_type")?.Value;

            if (!Enum.TryParse<BookerType>(claim, out var bookerType) || !Enum.IsDefined(bookerType))
            {
                throw new UnauthorizedAccessException("The token has no valid booker type.");
            }

            return bookerType;
        }
    }
}
```

This is the only class in the whole system that knows the booker type comes from a token.
It lives in the Web project, because it needs `HttpContext`, and that only exists there.

### Part by part

| Part | What it does |
|---|---|
| `: ICurrentUser` | This class is the working answer to the interface above |
| `IHttpContextAccessor` | Built into ASP.NET Core. It gives access to the request that is being handled right now |
| `HttpContext?.User` | The logged-in user. ASP.NET Core has already verified the token and filled this in |
| `.FindFirst("booker_type")?.Value` | Looks for the claim with that name and takes its text, for example `"Association"` |
| `Enum.TryParse<BookerType>(...)` | Turns the text into the enum. It gives `false` when the text is missing or not a booker type |
| `Enum.IsDefined(bookerType)` | Refuses numbers that are not a real booker type. `TryParse` alone accepts the text `"99"` |
| `throw new UnauthorizedAccessException(...)` | No valid booker type means no booking. There is no fallback to a default type |

The two question marks (`?.`) mean "if this is missing, carry on with null". A missing
request or a missing claim then ends up in the same place as a wrong one: `TryParse` gives
`false`, and the class throws.

### Why it throws and does not pick a default

A default would have to be one of the three types, and each of them has a price and a set of
venues it may book. Guessing `Private` could overcharge an association, and guessing
`Association` would give a private person half price. A token without a valid booker type is
a fault in the login, and the safe answer is to refuse.

## How it is connected

Two lines at startup tell ASP.NET Core which class answers the interface:

```csharp
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUser, HttpContextCurrentUser>();
```

`AddScoped` means one `HttpContextCurrentUser` per request, so two users booking at the same
moment never see each other's booker type.

## How the handler uses it

The handler in `ExampleBookingCommandHandler.md` takes `ICurrentUser` in its constructor, and
asks it who is booking on the line that calls the domain:

```csharp
var booking = Booking.Reserve(venue, _currentUser.BookerType, timeSlot, command.ParticipantCount, contact, now);
```

`ReserveBookingCommand` has no `BookerType` at all. The command holds only what the
user typed, so there is no field a user could fill in to claim another type.

## The same idea as `TimeProvider`

The handler already gets the time from a clock it is handed, and does not read the system
clock itself. `ICurrentUser` does the same for the user.

| Question | The handler asks | Real answer comes from | A test hands in |
|---|---|---|---|
| What time is it? | `TimeProvider` | The system clock | A fixed time |
| Who is booking? | `ICurrentUser` | The login token | A fake with a chosen booker type |

In both cases the domain gets a plain value: a `DateTime` and a `BookerType`.

## What exists in this repository and what does not

| Name in the example | Exists here | Where it would live |
|---|---|---|
| `BookerType` | Yes | Domain |
| `Booking.Reserve` | Yes | Domain |
| `ICurrentUser` | No | Application |
| `ReserveBookingCommandHandler` | No | Application |
| `HttpContextCurrentUser` | No | Web |
| `IHttpContextAccessor`, `HttpContext` | Built into ASP.NET Core | |

## How the handler would be tested

With a fake that answers whatever the test tells it to:

```csharp
public sealed class FakeCurrentUser : ICurrentUser
{
    public BookerType BookerType { get; set; } = BookerType.Private;
}
```

A handler test sets `BookerType.Region` on the fake and checks that reserving Sporthallen
Norr is refused. No token and no web request are needed.

## Three limits of the example

- **The token is a snapshot.** It says what the account was at login. If an account changes
  from private person to association, the old token says private until it runs out. A system
  where that matters would keep only the user id in the token and look the type up on every
  request.
- **It only works inside a web request.** A background job has no token, so a handler that
  uses `ICurrentUser` cannot be called from one. The job that would call `Expire` is not
  affected, because expiring needs no booker type.
- **The claim name is made up.** `booker_type` is whatever the login decides to call it. The
  name has to match in two places, the login and this class, and nothing checks that it does.
