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
Test cases goes in `TE.DomainStorytellingApplied.Domain.Tests` (naming: `WhenXYZ_ShouldXYZ`)

# Domain model
Code is organised in modules by domain concept: `Actors`, `Premises`, `Timeslots`, `Reservations`, `BookingPolicy`, `Notifications`, `Shared`.

| Building block | Types |
|---|---|
| Value Objects | `Id`, `ReservationStatus`, `MaxActiveReservations`, `PolicyViolation` |
| Entities / Aggregates | `Booker`, `Approver`, `Premises`, `Timeslot` (references `Premises` by `PremisesId`), `Reservation` (review lifecycle) |
| Domain Services | `ReservationService` (US1), `BookingPolicy` (US2), `ReservationReviewService` (US3), `ReservationNotifier` (US4), `TimeslotService` (no overlapping timeslots per premises) |
| Ports | `IReservationRepository`, `ITimeslotRepository`, `INotificationSender`, `TimeProvider` |

User stories:
1. Booker reserves an available Timeslot for Premises in the System, which creates a Reservation.
2. System checks the Booking Policy for Booker, Timeslot and Premises:
   - the timeslot is in the future
   - the timeslot is not already reserved
   - the booker has fewer than the maximum number of active reservations
3. Any Approver reviews the pending Reservation manually and approves or rejects it (once). A rejection releases the Timeslot.
4. System sends a Confirmation or Rejection to the Booker right after the review.