using TE.DomainStorytellingApplied.Domain.Bookers;

namespace TE.DomainStorytellingApplied.Domain.Tests.Bookers;

public class AssociationRepresentativeTests
{
    [Fact]
    public void GivenNullOrganizationName_WhenCreating_ShouldThrow()
    {
        Should.Throw<ArgumentNullException>(() => new AssociationRepresentative(null!));
    }

    [Fact]
    public void GivenValidOrganizationName_WhenCreating_ShouldSucceed()
    {
        var organizationName = new Name("IFK Göteborg");

        var type = new AssociationRepresentative(organizationName);

        type.OrganizationName.ShouldBe(organizationName);
    }

    [Fact]
    public void GivenAssociationRepresentative_WhenCreatingBooker_ShouldExposeOrganizationName()
    {
        var organizationName = new Name("IFK Göteborg");
        var booker = new Booker(
            new Name("Anna"),
            new Name("Andersson"),
            new PhoneNumber("070-1234567"),
            new Email("anna@example.com"),
            new AssociationRepresentative(organizationName));

        booker.Type.ShouldBeOfType<AssociationRepresentative>()
            .OrganizationName.ShouldBe(organizationName);
    }
}
