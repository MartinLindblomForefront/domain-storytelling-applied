using TE.DomainStorytellingApplied.Domain.Common;

namespace TE.DomainStorytellingApplied.Domain.Bookers;

public sealed record Name
{
    public string Value { get; }

    public Name(string value)
    {
        Guard.EnsureNotEmpty(value);

        Value = value;
    }

    public override string ToString() => Value;
}
