namespace TE.DomainStorytellingApplied.Domain;

/// <summary>
/// Recorded by a Booking when it is paid. It is named in the past tense because it is a fact.
/// It carries what a listener needs, for example to send a confirmation email with a receipt.
/// </summary>
public sealed record BookingConfirmed(Guid BookingId, string ContactEmail, Money Fee) : IDomainEvent;
