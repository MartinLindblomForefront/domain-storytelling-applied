namespace TE.DomainStorytellingApplied.Domain.Premises;

public sealed class InvalidPremiseException : DomainException
{
    private InvalidPremiseException(string message) : base(message) { }

    public static InvalidPremiseException EmptyId() => new("Premise id must not be empty.");

    public static InvalidPremiseException BlankName() => new("Premise name must not be blank.");
}
