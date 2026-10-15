namespace TE.DomainStorytellingApplied.Domain;

public sealed record Booker
{
    public Id Id { get; }
    public string Name { get; }

    public Booker(Id id, string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Booker name cannot be null or empty.", nameof(name));
        }

        Id = id;
        Name = name;
    }
}
