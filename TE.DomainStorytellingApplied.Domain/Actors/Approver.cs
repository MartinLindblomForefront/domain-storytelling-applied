namespace TE.DomainStorytellingApplied.Domain;

public sealed record Approver
{
    public Id Id { get; }
    public string Name { get; }

    public Approver(Id id, string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Approver name cannot be null or empty.", nameof(name));
        }

        Id = id;
        Name = name;
    }
}
