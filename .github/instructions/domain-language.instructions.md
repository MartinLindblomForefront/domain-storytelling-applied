---
description: Ubiquitous language and booking story from domainStory.png. Use when naming or modelling domain concepts.
applyTo: "**/*.cs"
---

# Ubiquitous Language

Source of truth: `domainStory.png`. Use these terms exactly in code, tests and names. Do not invent synonyms (no `User`, `Room`, `Booking`, `Slot`).

| Term | Kind | Notes |
|---|---|---|
| Booker | Actor | Municipality Employee, Association Representative or Private Individual |
| Approver | Actor | Reviews reservations that need review |
| System | Actor | Performs checks, routing, approval and notifications |
| Premises | Concept | What is reserved |
| Timeslot | Concept | When it is reserved |
| Booking Policy | Concept | Rules per Booker type, Timeslot and Premises; decides if review is required |
| Reservation | Concept | Created by the System when a Booker reserves an available Timeslot for Premises |
| Confirmation / Rejection | Concept | Sent to the Booker with the outcome |

# Story

1. Booker reserves an available Timeslot for Premises; System creates a Reservation.
2. System checks the Booking Policy (for Booker, Timeslot, Premises).
3. Exactly one of:
   - Policy requires no review: System approves the Reservation.
   - Policy requires review: System routes the Reservation to an Approver, who approves or rejects it.
4. System sends a Confirmation or Rejection to the Booker.
