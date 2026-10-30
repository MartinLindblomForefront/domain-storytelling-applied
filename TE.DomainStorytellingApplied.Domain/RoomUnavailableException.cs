namespace TE.DomainStorytellingApplied.Domain;

public class RoomUnavailableException() : InvalidOperationException(
    "Rummet är redan bokat för denna tid.");
