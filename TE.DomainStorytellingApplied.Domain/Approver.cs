namespace TE.DomainStorytellingApplied.Domain;

public sealed class Approver
{
    public Approver(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        Id = Guid.NewGuid();
        Name = name;
    }

    public Guid Id { get; }

    public string Name { get; }
}
