namespace TE.DomainStorytellingApplied.Domain;

public enum BookerType
{
    Private,
    Association,
    Municipality
}

public sealed record Booker
{
    public string Name { get; private init; }

    public BookerType Type { get; private init; }

    public Booker(string name, BookerType type)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new Exception("Booker must have a name");

        Name = name;
        Type = type;
    }
}
