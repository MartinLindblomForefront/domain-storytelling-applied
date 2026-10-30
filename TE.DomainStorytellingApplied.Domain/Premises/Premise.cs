namespace TE.DomainStorytellingApplied.Domain.Premises;

// Domain value object
public sealed record Premise
{
    public Guid Id { get; }
    public string Name { get; }

    public Premise(Guid id, string name)
    {
        if (id == Guid.Empty)
        {
            throw InvalidPremiseException.EmptyId();
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw InvalidPremiseException.BlankName();
        }

        Id = id;
        Name = name.Trim();
    }
}
