namespace TE.DomainStorytellingApplied.Domain.Tests.Bookers;

using TE.DomainStorytellingApplied.Domain.Bookers;

public class BookerTypeTests
{
    [Fact]
    public void GivenStoryBookers_WhenListingBookerTypes_ShouldContainExactlyMunicipalityEmployeeAssociationRepresentativePrivateIndividual()
    {
        Enum.GetValues<BookerType>().ShouldBe(
            [
                BookerType.MunicipalityEmployee,
                BookerType.AssociationRepresentative,
                BookerType.PrivateIndividual,
            ],
            ignoreOrder: true);
    }
}
