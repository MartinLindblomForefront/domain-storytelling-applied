namespace TE.DomainStorytellingApplied.Domain;

public sealed record Id
{
    public string Value { get; }

    public Id(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Id cannot be null or empty.", nameof(value));
        }

        Value = value;
    }

    public static Id New() => new(Guid.NewGuid().ToString());

    public override string ToString() => Value;
}
