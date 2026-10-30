# Arbetsplan – Bokning av lokaler

Arbetet är uppdelat i steg som följer domänberättelsen. Varje steg ska kunna implementeras och testas för sig innan nästa påbörjas.

## Struktur
Varje domänbegrepp får en egen mapp och namespace, speglat i testprojektet:
```
TE.DomainStorytellingApplied.Domain/
  Timeslots/Timeslot.cs
  Premises/...
  Bookers/...
  Reservations/...
TE.DomainStorytellingApplied.Domain.Tests/
  Timeslots/TimeslotTests.cs
  ...
```

## Steg 1 – Timeslot ✅
**Mål:** Ett tidsintervall kopplat till en lokal.
- `Id`, `PremisesId`, `StartTime`, `EndTime`
- Sluttid måste vara efter starttid

**Tester:**
- Sluttid före starttid → fel
- Sluttid lika med starttid → fel
- Giltigt intervall → skapas korrekt

## Steg 2 – Premises ✅
**Mål:** Lokalen som kan bokas (klass `Premise`).
- `Id`, `Name`
- Namn får inte vara tomt

**Tester:**
- Tomt/null/blanksteg-namn → fel
- Giltig lokal → skapas korrekt
- Två lokaler får olika `Id`

## Steg 3 – Booker ✅
**Mål:** Den som bokar.
- `Id`, `FirstName`, `LastName`, `PhoneNumber`, `Email`, `Type`
- `Type` injiceras som `IBookerType`: `MunicipalityEmployee`, `PrivateIndividual`, `AssociationRepresentative` (med `OrganizationName`)
- Inga textfält får vara tomma/null

**Tester:**
- Tomt/null/blanksteg i varje textfält → fel
- `null` som typ → fel
- Varje booker-typ kan sättas
- `AssociationRepresentative` utan organisationsnamn → fel
- Två bookers får olika `Id`

## Steg 4 – Reservation (skapande) ✅
**Mål:** Systemet skapar en reservation när en Booker reserverar en Timeslot.
- `Id`, `Booker`, `Timeslot`, `Premise`, `Status` (`Pending`/`Approved`)
- Ny reservation får status `Pending`
- Timesloten måste tillhöra den angivna lokalen

**Tester:**
- Ny reservation har status `Pending`
- Reservation refererar rätt Booker, Timeslot och Premise
- Två reservationer får olika `Id`
- `null` för Booker/Timeslot/Premise → fel
- Timeslot för annan lokal → fel

## Steg 5 – Reservation (statusövergångar)
**Mål:** Reservationen kan godkännas eller avslås, men bara från rätt status.
- `Approve()`, `Reject()`, ev. `RouteForReview()` → `AwaitingReview`
- Avslutad reservation (`Approved`/`Rejected`) kan inte ändras igen

**Tester:**
- `Pending` → `Approved` fungerar
- `AwaitingReview` → `Approved`/`Rejected` fungerar
- `Approved` → `Rejected` → fel (och vice versa)

## Steg 6 – Tillgänglighet
**Mål:** En Timeslot kan bara reserveras om den är ledig ("Reserves available").
- Kontroll att inga överlappande godkända/väntande reservationer finns för samma lokal

**Tester:**
- Överlappande tid i samma lokal → nekas
- Överlappande tid i annan lokal → tillåts
- Angränsande tider (10–12 och 12–14) → tillåts

## Steg 7 – BookingPolicy
**Mål:** Systemet kontrollerar policyn för Booker, Timeslot och Premises.
- Avgör om bokningen är **tillåten** och om den **kräver granskning**
- Exempel på regler (bekräftas av gruppen):
  - Privatpersoner kräver alltid granskning
  - Kommunanställda godkänns direkt
  - Max längd på en timeslot

**Tester:**
- En test per regel, både uppfyllt och ej uppfyllt

## Steg 8 – Approver
**Mål:** En granskare godkänner eller avslår reservationer som routats för granskning.
- `Approver` med `Id`, `Name`
- Kan bara granska reservationer med status `AwaitingReview`

**Tester:**
- Approver godkänner → `Approved`
- Approver avslår → `Rejected`
- Granska reservation som inte väntar på granskning → fel

## Steg 9 – Bekräftelse / Avslag
**Mål:** Systemet skickar Confirmation eller Rejection till Booker.
- Ett meddelande/händelse skapas när reservationen blir `Approved` eller `Rejected`

**Tester:**
- Godkänd reservation → Confirmation till rätt Booker
- Avslagen reservation → Rejection till rätt Booker

## Steg 10 – Hela flödet (System)
**Mål:** Knyta ihop stegen till hela berättelsen.
1. Booker reserverar ledig Timeslot för Premises → Reservation skapas
2. BookingPolicy kontrolleras
3. Godkänns direkt **eller** routas till Approver
4. Confirmation/Rejection skickas till Booker

**Tester:**
- Hela flödet utan granskning
- Hela flödet med granskning (godkänd)
- Hela flödet med granskning (avslagen)
- Upptagen timeslot → nekas direkt

## Öppna frågor
- Vilka konkreta regler ska BookingPolicy innehålla?
- Ska en avslagen reservation frigöra tiden direkt?
- Ska en Booker kunna avboka?
