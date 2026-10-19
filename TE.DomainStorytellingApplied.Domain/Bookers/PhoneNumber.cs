using TE.DomainStorytellingApplied.Domain.Common;

namespace TE.DomainStorytellingApplied.Domain.Bookers;

public sealed record PhoneNumber
{
    public string Value { get; }

    public PhoneNumber(string value)
    {
        Guard.EnsureNotEmpty(value);

        Value = value;
    }

    public override string ToString() => Value;
}
