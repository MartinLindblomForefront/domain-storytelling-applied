namespace TE.DomainStorytellingApplied.Domain;

public sealed class Premises
{
    public Id Id { get; }
    public string Name { get; }

    public Premises(Id id, string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Premises name cannot be null or empty.", nameof(name));
        }

        Id = id;
        Name = name;
    }
}
