using TE.DomainStorytellingApplied.Domain.Aggregates.Bookers.ValueObjects;

namespace TE.DomainStorytellingApplied.Domain.Aggregates.Bookers.AggregateRoots;

public sealed class Booker : IEquatable<Booker>
{
    public Booker(BookerId id, string name)
    {
        ArgumentNullException.ThrowIfNull(id);

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Booker name cannot be empty.", nameof(name));
        }

        Id = id;
        Name = name;
    }

    public BookerId Id { get; }

    public string Name { get; }

    public bool Equals(Booker? other)
    {
        return other is not null && Id == other.Id;
    }

    public override bool Equals(object? obj)
    {
        return obj is Booker other && Equals(other);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}
