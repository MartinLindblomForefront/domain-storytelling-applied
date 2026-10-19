namespace TE.DomainStorytellingApplied.Domain.Bookers;

public sealed class AssociationRepresentative : IBookerType
{
    public Name OrganizationName { get; private init; }

    public AssociationRepresentative(Name organizationName)
    {
        ArgumentNullException.ThrowIfNull(organizationName);

        OrganizationName = organizationName;
    }
}
