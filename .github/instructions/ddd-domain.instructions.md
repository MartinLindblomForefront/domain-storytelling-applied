---
description: DDD rules for the domain model. Use when adding or changing code in the Domain project.
applyTo: "TE.DomainStorytellingApplied.Domain/**/*.cs"
---

# DDD Rules

- The Domain project is pure: no infrastructure, UI, persistence or framework dependencies.
- `Reservation` is the aggregate root. Other objects are reached through it; reference other aggregates by id.
- Entities have identity; `Timeslot`, `Premises` identifiers and similar are immutable value objects (`record`/`readonly record struct`).
- Keep invariants inside the model: private setters, factory methods or constructors that validate, no public mutable collections.
- Express the story as behaviour on the model (e.g. `Approve()`, `Reject()`, `RouteForReview()`), not as setters or anemic services.
- Model Reservation state explicitly (e.g. Requested, PendingReview, Approved, Rejected) and reject invalid transitions with a domain exception.
- Broken business rules throw a concept-specific exception deriving from `DomainException` (e.g. `InvalidTimeslotException`, `InvalidPremiseException`), placed in the concept's folder. Messages live only in the exception: private constructor plus named static factories (e.g. `InvalidPremiseException.BlankName()`). Never `throw new ArgumentException("...")` for a rule; `ArgumentException` is only for programming errors.
- Mark value objects with the one-line comment `// Domain value object` above the type, policies with `// Domain policy`, and the aggregate root with `// Domain aggregate root`.
- `Reservation.Evaluate(ApprovalPolicy)` moves Requested to Approved or PendingReview. `Approve()`/`Reject()` (the Approver) only work from PendingReview, so the policy cannot be bypassed.
- `BookingPolicy` is a domain object/specification that decides whether a Reservation requires review. No `if` on Booker type spread across the code.
- `ApprovalPolicy` turns the `BookingPolicy` decision into an `ApprovalRoute` (`AutoApprove` or `RouteForReview`), the OR branch in the story. Current rules: Municipality Employee never needs review, Association Representative only above 4 hours, Private Individual always.
- The System's actions that span objects (check policy, route, notify) go in a domain service or application layer; notifications are a port (interface), not an implementation.
- Raise domain events (e.g. `ReservationApproved`, `ReservationRejected`) instead of calling out directly.
- Folder per concept/aggregate, not per technical type.
