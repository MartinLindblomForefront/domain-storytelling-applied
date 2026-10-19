using TE.DomainStorytellingApplied.Domain.Common;

namespace TE.DomainStorytellingApplied.Domain.Bookers;

public sealed record Email
{
    public string Value { get; }

    public Email(string value)
    {
        Guard.EnsureNotEmpty(value);

        Value = value;
    }

    public override string ToString() => Value;
}
