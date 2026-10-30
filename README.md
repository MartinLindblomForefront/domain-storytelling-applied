# Requirements
```sh
dotnet --version 
```
Should be 10 or higher. If not install the dotnet 10 SDK.

# Getting the code
```sh
git clone https://github.com/MartinLindblomForefront/domain-storytelling-applied.git
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

# The booking PoC
A minimal booking system (MVP) for municipal venues (classrooms, halls, pitches).
There is no UI, API or database. The only way to run it is through the tests.

## Design in short
| Type | What it is |
|---|---|
| `Venue` | Aggregate root. Owns its `Booking`s and protects "no double booking" |
| `Booking` | Entity inside `Venue`. Reserved, then Confirmed |
| `TimeSlot`, `Money`, ids | Value objects. `BookerType` is an enum |

## Rules (assumptions, not verified requirements)
- A slot is one whole hour, within opening hours and never in the past
- The same slot can not be booked twice
- Municipality pays 0 kr and is confirmed directly. Others pay the hourly rate
- A reservation holds the slot for 15 minutes. `ConfirmBooking` (payment done) must come within that time, otherwise the slot is free again

Not included: cancellation, refunds, late payments, multi-hour bookings, concurrent bookings, storage.