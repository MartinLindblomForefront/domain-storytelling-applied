namespace TE.DomainStorytellingApplied.Domain;

public readonly record struct VenueId(Guid Value)
{
    public static VenueId New()
    {
        return new VenueId(Guid.NewGuid());
    }
}

public readonly record struct BookingId(Guid Value)
{
    public static BookingId New()
    {
        return new BookingId(Guid.NewGuid());
    }
}
