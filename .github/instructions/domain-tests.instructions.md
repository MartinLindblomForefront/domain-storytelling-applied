---
description: Testing conventions for the domain model. Use when writing or changing tests.
applyTo: "TE.DomainStorytellingApplied.Domain.Tests/**/*.cs"
---

# Tests

- xUnit. One test class per aggregate or domain service, mirroring the Domain folder structure.
- Test behaviour from the story, not implementation details. Name tests after the story step, e.g. `Reservation_is_approved_when_policy_requires_no_review`.
- Cover both branches of the OR: automatic approval and routed review (approved and rejected).
- Cover invalid state transitions and invariants. Assert the specific exception type (e.g. `Should.Throw<InvalidPremiseException>`), not `ArgumentException`, and not message text.
- Use the ubiquitous language in names and variables.
- Arrange-Act-Assert; no mocks of domain objects, only of ports (e.g. notification).
- Write the failing test first, then the minimal domain code.
