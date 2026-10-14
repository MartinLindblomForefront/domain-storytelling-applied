namespace TE.DomainStorytellingApplied.Domain;

public sealed record Id
{
    public string Value { get; private init; }

    private Id(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new Exception("Id may not be null or empty");

        Value = value;
    }

    public static Id Generate() => new(Guid.NewGuid().ToString());

    public static Id From(string value) => new(value);
}
