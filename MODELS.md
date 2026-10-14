# Modeller – Bokningstjänst

Här beskrivs de modeller som behövs i domänen: vad de innehåller, vilka metoder de har, vilka regler som gäller och vilka testfall som verifierar reglerna.

Ogiltiga värden ger `ArgumentException` eller `ArgumentNullException`. Otillåtna åtgärder, till exempel fel status, nekad policy eller överlapp, ger `InvalidOperationException`.

## Roller i domänen

| Modell | Roll |
|---|---|
| `Booker` | Entitet, eget aggregat |
| `Approver` | Entitet, eget aggregat |
| `Premises` | Aggregatrot som äger sina bokningar |
| `Reservation` | Entitet inuti `Premises`-aggregatet |
| `Timeslot` | Värdeobjekt |
| `BookingPolicy` | Värdeobjekt |
| `Notification` | Besked som härleds från en bokning |

Aggregat refererar till varandra med id, inte med objektreferenser.

---

## Booker (Bokare)

| Egenskap | Typ | Beskrivning |
|---|---|---|
| `Id` | `Guid` | Unik identitet |
| `Name` | `string` | Bokarens namn |
| `Email` | `string` | E-postadress som beskeden skickas till |
| `Type` | `BookerType` | `MunicipalityEmployee`, `AssociationRepresentative` eller `PrivateIndividual` |

**Regler och testfall**
- Namn får inte vara tomt.
- E-postadressen får inte vara tom och måste ha ett giltigt format.
- Bokartypen måste vara ett giltigt värde.

---

## Approver (Godkännare)

| Egenskap | Typ | Beskrivning |
|---|---|---|
| `Id` | `Guid` | Unik identitet |
| `Name` | `string` | Godkännarens namn |

**Regler och testfall**
- Namn får inte vara tomt.

---

## Premises (Lokal)

Aggregatrot. Är den enda vägen att skapa, godkänna och avslå bokningar, så att policyn och överlapp alltid kontrolleras.

| Egenskap | Typ | Beskrivning |
|---|---|---|
| `Id` | `Guid` | Unik identitet |
| `Name` | `string` | Lokalens namn |
| `BookingPolicy` | `BookingPolicy` | Policyn som gäller för lokalen |
| `Reservations` | `IReadOnlyCollection<Reservation>` | Lokalens bokningar |

**Metoder**

| Metod | Beskrivning |
|---|---|
| `Reserve(Booker booker, Timeslot timeslot, DateTimeOffset now)` | Skapar en bokning om policyn tillåter och tidsluckan är ledig |
| `Approve(Guid reservationId, Approver approver)` | Godkänner en bokning som väntar på granskning |
| `Reject(Guid reservationId, Approver approver, string reason)` | Avslår en bokning som väntar på granskning |

**Regler och testfall**
- Namn får inte vara tomt.
- En lokal måste ha en bokningspolicy.
- En bokning som policyn nekar skapas inte.
- En tidslucka som överlappar en befintlig, ej avslagen bokning kan inte bokas.
- En tidslucka som överlappar en avslagen bokning kan bokas.
- Två tidsluckor som ligger direkt efter varandra kan bokas.
- Godkännande eller avslag av en okänd bokning ger `ArgumentException`.

---

## Timeslot (Tidslucka)

Värdeobjekt.

| Egenskap | Typ | Beskrivning |
|---|---|---|
| `Start` | `DateTimeOffset` | Starttid med tidszonsförskjutning |
| `End` | `DateTimeOffset` | Sluttid med tidszonsförskjutning |
| `Duration` | `TimeSpan` | Beräknas som `End - Start` |

**Metoder**

| Metod | Beskrivning |
|---|---|
| `Overlaps(Timeslot other)` | Returnerar `true` om tidsluckorna överlappar |

**Regler och testfall**
- Starttiden får inte vara senare än sluttiden.
- Starttiden får inte vara lika med sluttiden, eftersom tidsluckan då saknar längd.
- `Duration` ska vara `End - Start`.
- Två tidsluckor som överlappar ska upptäckas (`Overlaps`).
- Två tidsluckor som ligger direkt efter varandra (A slutar 10:00, B börjar 10:00) överlappar inte.
- Två tidsluckor med samma start- och sluttid ska vara lika (värdelikhet).
- Överlapp jämförs på faktisk tidpunkt, även om tidsluckorna har olika tidszonsförskjutning.

---

## BookingPolicy (Bokningspolicy)

Värdeobjekt. Två policyer med samma värden är lika, oavsett ordning på de tillåtna bokartyperna.

| Egenskap | Typ | Beskrivning |
|---|---|---|
| `AllowedBookerTypes` | `IReadOnlyCollection<BookerType>` | Vilka bokartyper som får boka |
| `RequiresReview` | `bool` | Om bokningar kräver manuell granskning |
| `MinDuration` | `TimeSpan` | Kortaste tillåtna bokningslängd |
| `MaxDuration` | `TimeSpan` | Längsta tillåtna bokningslängd |
| `OpensAt` | `TimeOnly` | Tid då lokalen öppnar |
| `ClosesAt` | `TimeOnly` | Tid då lokalen stänger |

**Metoder**

| Metod | Beskrivning |
|---|---|
| `Allows(Booker booker, Timeslot timeslot, DateTimeOffset now)` | Returnerar `true` om bokaren får boka tidsluckan |

**Regler och testfall**
- `MinDuration` måste vara större än 0.
- `MinDuration` får inte
- Öppningstiden måste vara tidigare än stängningstiden.
- Minst en bokartyp måste vara tillåten.
- Alla tillåtna bokartyper måste vara giltiga värden.
- En bokare vars typ inte är tillåten nekas.
- En tidslucka som är kortare än `MinDuration` nekas.
- En tidslucka som är längre än `MaxDuration` nekas.
- En tidslucka utanför öppettiderna nekas. Tidsluckan måste börja och sluta samma dag. Öppettiderna jämförs med tidsluckans lokala klocktid, det vill säga tiden i tidsluckans egen tidszonsförskjutning.
- En tidslucka i det förflutna nekas.
- Policyn anger om bokningen kräver granskning.

---

## Reservation (Bokning)

Entitet inuti `Premises`-aggregatet. Skapas endast via `Premises.Reserve`. Konstruktorn och alla metoder är `internal`.

| Egenskap | Typ | Beskrivning |
|---|---|---|
| `Id` | `Guid` | Unik identitet |
| `BookerId` | `Guid` | Id för den som bokar |
| `PremisesId` | `Guid` | Id för lokalen som bokas |
| `Timeslot` | `Timeslot` | Tidsluckan som bokas |
| `Status` | `ReservationStatus` | `Created`, `PendingReview`, `Approved` eller `Rejected` |
| `ReviewedById` | `Guid?` | Id för godkännaren, om bokningen har granskats |
| `RejectionReason` | `string?` | Anledning till avslag |

**Metoder**

| Metod | Synlighet | Beskrivning |
|---|---|---|
| `Submit(bool requiresReview)` | `internal` | Flyttar bokningen från `Created` till `Approved` eller `PendingReview` |
| `Approve(Approver approver)` | `internal` | Godkänner en bokning som väntar på granskning |
| `Reject(Approver approver, string reason)` | `internal` | Avslår en bokning som väntar på granskning |

**Statusflöde**

```
Created ──(policyn kräver inte granskning)──► Approved
Created ──(policyn kräver granskning)──► PendingReview ──► Approved | Rejected
```

**Regler och testfall**
- En ny bokning har statusen `Created`.
- Bokar-id och lokal-id får inte vara tomma, och tidsluckan är obligatorisk.
- Bokningen går till `Approved` om policyn inte kräver granskning.
- Bokningen går till `PendingReview` om policyn kräver granskning.
- Endast en bokning med statusen `PendingReview` kan godkännas eller avslås av en godkännare.
- Ett avslag kräver en anledning.
- En bokning som har statusen `Approved` eller `Rejected` kan inte ändras.
- `ReviewedById` sätts när en godkännare godkänner eller avslår bokningen.

---

## Notification (Besked)

| Egenskap | Typ | Beskrivning |
|---|---|---|
| `RecipientId` | `Guid` | Id för mottagaren (bokaren) |
| `ReservationId` | `Guid` | Bokningen som beskedet gäller |
| `Type` | `NotificationType` | `Confirmation` eller `Rejection` |
| `Message` | `string` | Meddelandetext, inklusive anledningen vid avslag |

**Metoder**

| Metod | Beskrivning |
|---|---|
| `static For(Reservation reservation)` | Skapar ett besked för en godkänd eller avslagen bokning |

**Regler och testfall**
- En godkänd bokning ger ett besked av typen `Confirmation`.
- En avslagen bokning ger ett besked av typen `Rejection` som innehåller anledningen.
- Inget besked skapas för en bokning som har statusen `Created` eller `PendingReview`.
