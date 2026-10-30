using TE.DomainStorytellingApplied.Domain.Common;

namespace TE.DomainStorytellingApplied.Domain.Bookers;

public sealed class AssociationRepresentative : IBookerType
{
    public string OrganizationName { get; private init; }

    public AssociationRepresentative(string organizationName)
    {
        Guard.EnsureNotEmpty(organizationName);

        OrganizationName = organizationName;
    }
}
