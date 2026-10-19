using TE.DomainStorytellingApplied.Domain.Common;

namespace TE.DomainStorytellingApplied.Domain.Premises;

public class Premise
{
    public Guid Id { get; private init; }
    public string Name { get; private init; }

    public Premise(string name)
    {
        Guard.EnsureNotEmpty(name);

        Id = Guid.NewGuid();
        Name = name;
    }
}
