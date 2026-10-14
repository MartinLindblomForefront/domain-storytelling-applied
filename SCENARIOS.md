# Scenarier – Bokningstjänst

Scenarierna nedan är hämtade från domänberättelsen och används som utgångspunkt för testerna.

## Aktörer

- **Bokare (Booker)** – kan vara en kommunanställd, en föreningsrepresentant eller en privatperson.
- **Godkännare (Approver)** – granskar bokningar som kräver manuellt godkännande.
- **System** – tar emot bokningar, kontrollerar bokningspolicyn och skickar besked.

## Arbetsobjekt

- **Tidslucka (Timeslot)**
- **Lokal (Premises)**
- **Bokning (Reservation)**
- **Bokningspolicy (Booking Policy)**
- **Bekräftelse eller avslag (Confirmation or Rejection)**

Modellerna och deras regler beskrivs i detalj i [MODELS.md](MODELS.md).

---

## Scenario 0 – Giltiga modeller

Innan en bokning kan göras måste de ingående modellerna vara giltiga.

**Exempel på testfall**
- En tidslucka kan inte skapas med en starttid som är senare än sluttiden.
- En tidslucka kan inte skapas med en starttid som är lika med sluttiden.
- En lokal kan inte skapas utan namn eller utan bokningspolicy.
- En bokare kan inte skapas utan namn eller med en ogiltig e-postadress.
- En bokningspolicy kan inte ha en `MinDuration` som är större än `MaxDuration`.
- En bokningspolicy måste tillåta minst en bokartyp.

## Scenario 1

1. Bokaren reserverar en ledig **tidslucka** för en **lokal** i systemet.
2. Systemet skapar en **bokning**.

**Exempel på testfall**
- En bokning skapas när bokaren reserverar en ledig tidslucka för en lokal.
- En tidslucka som redan är bokad kan inte reserveras igen.
- En tidslucka som överlappar en befintlig bokning kan inte reserveras.
- En tidslucka som börjar exakt när en annan bokning slutar kan reserveras.
- En tidslucka som överlappar en avslagen bokning kan reserveras.

## Scenario 2 – Systemet kontrollerar bokningspolicyn

1. Systemet kontrollerar **bokningspolicyn** för:
   - **bokaren**,
   - **tidsluckan** och
   - **lokalen**.

**Exempel på testfall**
- Policyn avgör om bokaren får boka lokalen.
- Policyn avgör om tidsluckan är tillåten för lokalen.
- Policyn avgör om bokningen kräver granskning.
- En bokare vars typ inte är tillåten nekas.
- En tidslucka som är kortare än `MinDuration` eller längre än `MaxDuration` nekas.
- En tidslucka utanför lokalens öppettider nekas.
- En tidslucka i det förflutna nekas.

## Scenario 3a – Automatiskt godkännande

*Gäller när bokningspolicyn inte kräver granskning.*

1. Systemet **godkänner** bokningen.

**Exempel på testfall**
- En bokning som inte kräver granskning godkänns direkt.

## Scenario 3b – Manuell granskning

*Gäller när bokningspolicyn kräver granskning.*

1. Systemet **skickar** bokningen till en **godkännare** för granskning.
2. Godkännaren **godkänner** eller **avslår** bokningen.

**Exempel på testfall**
- En bokning som kräver granskning får statusen "väntar på granskning".
- Godkännaren kan godkänna en bokning som väntar på granskning.
- Godkännaren kan avslå en bokning som väntar på granskning.
- En bokning som redan är godkänd eller avslagen kan inte granskas igen.
- Ett avslag kräver en anledning.
- Godkännaren registreras på bokningen när den granskas.

> Scenario 3a och 3b utesluter varandra. Endast ett av dem inträffar, beroende på om bokningspolicyn kräver granskning eller inte.

## Scenario 4 – Besked till bokaren

1. Systemet skickar en **bekräftelse** eller ett **avslag** till **bokaren**.

**Exempel på testfall**
- En bekräftelse skickas till bokaren när bokningen godkänns.
- Ett avslag skickas till bokaren när bokningen avslås.
- Avslaget innehåller anledningen.
- Inget besked skickas för en bokning som väntar på granskning.
