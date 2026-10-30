using TE.DomainStorytellingApplied.Domain.Bookers;

namespace TE.DomainStorytellingApplied.Domain.Tests.Bookers;

public class AssociationRepresentativeTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void GivenEmptyOrganizationName_WhenCreating_ShouldThrow(string? organizationName)
    {
        Should.Throw<ArgumentException>(() => new AssociationRepresentative(organizationName!));
    }

    [Fact]
    public void GivenValidOrganizationName_WhenCreating_ShouldSucceed()
    {
        var type = new AssociationRepresentative("IFK Göteborg");

        type.OrganizationName.ShouldBe("IFK Göteborg");
    }

    [Fact]
    public void GivenAssociationRepresentative_WhenCreatingBooker_ShouldExposeOrganizationName()
    {
        var booker = new Booker("Anna", "Andersson", "070-1234567", "anna@example.com",
            new AssociationRepresentative("IFK Göteborg"));

        booker.Type.ShouldBeOfType<AssociationRepresentative>()
            .OrganizationName.ShouldBe("IFK Göteborg");
    }
}
