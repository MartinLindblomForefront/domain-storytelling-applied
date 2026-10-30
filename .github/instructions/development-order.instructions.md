---
description: Layer structure, dependency rules and development order. Use when planning or adding new features, projects or files.
applyTo: "**/*.cs"
---

# Layers

`UI` → `Application` → `Domain` ← `Infrastructure`

- Domain depends on nothing. Application depends on Domain. Infrastructure implements ports from Domain/Application. UI depends on Application.
- Each layer is its own project: `TE.DomainStorytellingApplied.{UI,Application,Domain,Infrastructure}`. Create a project only when its step is reached.

# Development order (inside out, test first)

1. Domain value objects: `Timeslot`, `Premise`, `ReservationStatus`.
2. Domain policies: `BookingPolicy`, `ApprovalPolicy`.
3. Domain aggregate: `Reservation` (create, approve, reject, route for review).
4. Ports: `IReservationRepository`, `INotificationService`.
5. Application: `CreateReservationCommand`, `ReviewReservationCommand` and handlers (test with fakes).
6. Infrastructure: in-memory `ReservationRepository` first, then `EmailService`, `NotificationService`, `EntraIdProvider`.
7. UI: Reservation API endpoints and DI composition.

# Rules

- Finish a step (build and tests green) before starting the next.
- Do not add code for a later step before it is needed.
