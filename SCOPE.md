# Projektets omfattning

## Syfte

Målet med det här repot är att skapa en **domänmodell för en bokningstjänst**. All funktionalitet verifieras genom att skriva **tester**.

## Vad vi ska göra

- Modellera bokningstjänstens domän (entiteter, värdeobjekt, regler och beteenden) i `TE.DomainStorytellingApplied.Domain`.
- Verifiera allt beteende med tester i `TE.DomainStorytellingApplied.Domain.Tests`.
- Låta testerna fungera som körbar specifikation av domänen.

## Vad vi inte ska göra

- Inget API
- Ingen konsolapp
- Inget UI
- Ingen Blazor-applikation

## Arbetssätt

1. Beskriv ett scenario från domänberättelsen.
2. Skriv ett test som uttrycker det förväntade beteendet.
3. Implementera minsta möjliga kod i domänmodellen för att få testet att passera.
4. Refaktorera vid behov.
