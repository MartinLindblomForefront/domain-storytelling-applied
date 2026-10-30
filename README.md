# Domain Storytelling Applied

Det här projektet är en liten övning i taktisk Domain-Driven Design (DDD). Domänen handlar om att boka en lokal under en bestämd tidsperiod.

Projektet innehåller ingen applikation, databas eller webb-API. Fokus ligger på domänklasser och körbara tester.

## Krav

.NET 10 SDK eller senare krävs.

```sh
dotnet --version
```

## Bygga och testa

Kör kommandona från projektets rot:

```sh
dotnet build
dotnet test
```

Testerna är skrivna med xUnit och använder Shouldly för assertions.

## Projektstruktur

```text
TE.DomainStorytellingApplied.Domain/
├── Aggregates/
│   ├── Bookers/
│   │   ├── AggregateRoots/
│   │   │   └── Booker.cs
│   │   └── ValueObjects/
│   │       └── BookerId.cs
│   ├── Bookings/
│   │   ├── AggregateRoots/
│   │   │   └── Booking.cs
│   │   └── ValueObjects/
│   │       ├── BookingId.cs
│   │       ├── BookingStatus.cs
│   │       └── TimeSlot.cs
│   └── Venues/
│       ├── AggregateRoots/
│       │   └── Venue.cs
│       └── ValueObjects/
│           └── VenueId.cs
└── DomainServices/
    └── BookingAvailabilityService.cs

TE.DomainStorytellingApplied.Domain.Tests/
├── Aggregates/
│   ├── Bookers/
│   ├── Bookings/
│   └── Venues/
└── DomainServices/
    └── BookingAvailabilityServiceTests.cs
```

## Domänmodellen

Modellen består för närvarande av tre Aggregates:

```text
Booker Aggregate
└── Booker – Aggregate Root och Entity
    ├── BookerId – Value Object
    └── Name

Booking Aggregate
└── Booking – Aggregate Root och Entity
    ├── BookingId – Value Object
    ├── BookerId – referens till en Booker
    ├── VenueId – referens till en Venue
    ├── TimeSlot – Value Object
    └── BookingStatus – enum

Venue Aggregate
└── Venue – Aggregate Root och Entity
    ├── VenueId – Value Object
    ├── Name
    └── Capacity

BookingAvailabilityService – Domain Service
```

### Booker

`Booker` representerar personen som gör en bokning. Den är både Entity och Aggregate Root och identifieras av sitt `BookerId`.

Bokarens namn får inte vara tomt eller bara innehålla blanksteg. Två `Booker`-objekt med samma `BookerId` betraktas som samma person, även om namnet skiljer sig.

En `Booking` innehåller inte hela `Booker`-objektet utan refererar till bokaren genom `BookerId`. Det gör att bokaren och bokningen kan ha separata livscykler.

### Booking

`Booking` är både Entity och Aggregate Root för en bokning. Den identifieras av sitt `BookingId`, refererar till bokaren genom `BookerId`, refererar till en lokal genom `VenueId` och innehåller bokningens `TimeSlot`.

En ny bokning får alltid status `Pending`. Metoden `Confirm()` ändrar status till `Confirmed`. En redan bekräftad bokning kan inte bekräftas igen.

Två `Booking`-objekt med samma `BookingId` betraktas som samma Entity, även om deras övriga tillstånd skiljer sig.

### Venue

`Venue` betyder lokal och är både Entity och Aggregate Root för en lokal. Den har ett `VenueId`, ett namn och en kapacitet.

Följande regler skyddas av `Venue`:

- Namnet får inte vara tomt eller bara innehålla blanksteg.
- Kapaciteten måste vara större än noll.

Två `Venue`-objekt med samma `VenueId` betraktas som samma Entity. Identiteten avgör alltså vilken lokal det är, inte namnet eller kapaciteten.

### BookerId, BookingId och VenueId

`BookerId`, `BookingId` och `VenueId` är Value Objects som kapslar in var sitt `Guid`.

De är inte Entities. Ett ID har ingen egen livscykel utan representerar identiteten hos en Entity. Två ID-objekt med samma `Guid` är lika. `Guid.Empty` accepteras inte.

### TimeSlot

`TimeSlot` är ett oföränderligt Value Object med en start- och sluttid.

Det skyddar regeln att starttiden alltid måste vara före sluttiden. Två `TimeSlot` med samma start- och sluttid är lika.

Metoden `OverlapsWith()` avgör om två tidsperioder överlappar. Tidsperioder som endast möts vid samma tidpunkt överlappar inte. Det innebär exempelvis att en bokning som slutar 11:00 inte blockerar en ny bokning som börjar 11:00.

### BookingStatus

`BookingStatus` är en enum med två tillstånd:

- `Pending` – bokningen väntar på bekräftelse.
- `Confirmed` – bokningen är bekräftad.

### BookingAvailabilityService

`BookingAvailabilityService` är en Domain Service som kontrollerar om en lokal är ledig under en önskad tidsperiod.

Tjänsten tar emot:

- lokalens `VenueId`
- önskad `TimeSlot`
- befintliga bokningar

Lokalen är inte ledig om det finns en befintlig bokning med samma `VenueId` och en överlappande `TimeSlot`. Bokningar för andra lokaler påverkar inte resultatet.

Kontrollen ligger i en Domain Service eftersom den behöver jämföra flera bokningar. Själva regeln för tidsöverlappning ligger däremot i `TimeSlot`, eftersom den regeln hör till tidsperioden.

## Tester

Teststrukturen speglar domänprojektets struktur. Testnamnen använder formen `Given_When_Should` för att visa utgångsläge, handling och förväntat resultat.

### BookerTests

Verifierar att:

- en bokare kan skapas med giltigt ID och namn
- tomt namn och blanksteg nekas
- bokare med samma `BookerId` är samma Entity
- bokare med olika ID:n inte är samma Entity

### BookerIdTests

Verifierar att:

- ett giltigt `Guid` kan användas
- `Guid.Empty` nekas
- två `BookerId` med samma värde är lika
- två `BookerId` med olika värden inte är lika

### BookingTests

Verifierar att:

- en giltig bokning kan skapas
- bokningen refererar till rätt bokare genom `BookerId`
- en ny bokning får status `Pending`
- en väntande bokning kan bekräftas
- en bekräftad bokning inte kan bekräftas igen
- bokningar med samma `BookingId` är samma Entity
- bokningar med olika ID:n inte är samma Entity

### BookingIdTests

Verifierar att:

- ett giltigt `Guid` kan användas
- `Guid.Empty` nekas
- två `BookingId` med samma värde är lika
- två `BookingId` med olika värden inte är lika

### TimeSlotTests

Verifierar att:

- start före slut skapar en giltig tidsperiod
- samma start- och sluttid nekas
- start efter slut nekas
- två tidsperioder med samma värden är lika
- två tidsperioder med olika värden inte är lika
- överlappande tidsperioder identifieras
- tidsperioder som bara möts vid start och slut inte överlappar

### VenueTests

Verifierar att:

- en lokal kan skapas med giltiga uppgifter
- tomt namn och blanksteg nekas
- noll och negativ kapacitet nekas
- lokaler med samma `VenueId` är samma Entity
- lokaler med olika ID:n inte är samma Entity

### VenueIdTests

Verifierar att:

- ett giltigt `Guid` kan användas
- `Guid.Empty` nekas
- två `VenueId` med samma värde är lika
- två `VenueId` med olika värden inte är lika

### SanityTests

Innehåller ett enkelt kontrolltest som visar att testprojektet och Shouldly fungerar.

### BookingAvailabilityServiceTests

Verifierar att:

- en lokal utan befintliga bokningar är ledig
- en bokning för en annan lokal inte blockerar
- samma lokal är ledig när tiderna inte överlappar
- samma lokal inte är ledig när tiderna överlappar

## Det som ännu inte ingår

Modellen är medvetet liten. Följande delar är inte implementerade ännu:

- extra lokaler i en bokning
- betalning
- domänhändelser
- repositories och lagring
- applikationslager eller användargränssnitt
