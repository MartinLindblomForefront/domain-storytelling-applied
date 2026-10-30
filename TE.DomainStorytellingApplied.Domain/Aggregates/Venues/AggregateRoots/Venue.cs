
using TE.DomainStorytellingApplied.Domain.Aggregates.Venues.ValueObjects;

namespace TE.DomainStorytellingApplied.Domain.Aggregates.Venues.AggregateRoots;

public sealed class Venue : IEquatable<Venue>
{
    public Venue(VenueId id, string name, int capacity)
    {
        ArgumentNullException.ThrowIfNull(id);

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Venue name cannot be empty.", nameof(name));
        }

        if (capacity <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(capacity),
                capacity,
                "Venue capacity must be greater than zero.");
        }

        Id = id;
        Name = name;
        Capacity = capacity;
    }

    public VenueId Id { get; }

    public string Name { get; }

    public int Capacity { get; }

    public bool Equals(Venue? other)
    {
        return other is not null && Id == other.Id;
    }

    public override bool Equals(object? obj)
    {
        return obj is Venue other && Equals(other);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}
