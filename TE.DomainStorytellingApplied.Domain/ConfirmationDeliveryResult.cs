namespace TE.DomainStorytellingApplied.Domain;

public enum ConfirmationDeliveryStatus
{
    NotRequested,
    Pending,
    Sent,
    Failed,
    Suppressed
}

public sealed record ConfirmationDeliveryResult(ConfirmationDeliveryStatus Status, string? Error = null);
