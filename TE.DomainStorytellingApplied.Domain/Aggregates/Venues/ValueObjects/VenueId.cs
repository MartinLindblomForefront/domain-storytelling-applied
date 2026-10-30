
namespace TE.DomainStorytellingApplied.Domain.Aggregates.Venues.ValueObjects;

public sealed record VenueId
{
    public VenueId(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException("Venue ID cannot be empty.", nameof(value));
        }

        Value = value;
    }

    public Guid Value { get; }
}
