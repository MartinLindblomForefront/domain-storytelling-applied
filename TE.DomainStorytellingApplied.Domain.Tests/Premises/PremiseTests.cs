using TE.DomainStorytellingApplied.Domain.Premises;

namespace TE.DomainStorytellingApplied.Domain.Tests.Premises;

public class PremiseTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void GivenEmptyName_WhenCreatingPremise_ShouldThrow(string? name)
    {
        Should.Throw<ArgumentException>(() => new Premise(name!));
    }

    [Fact]
    public void GivenValidName_WhenCreatingPremise_ShouldSucceed()
    {
        var premise = new Premise("Gymnastiksalen");

        premise.Name.ShouldBe("Gymnastiksalen");
        premise.Id.ShouldNotBe(Guid.Empty);
    }

    [Fact]
    public void GivenTwoPremises_WhenCreated_ShouldHaveUniqueIds()
    {
        var first = new Premise("Gymnastiksalen");
        var second = new Premise("Gymnastiksalen");

        first.Id.ShouldNotBe(second.Id);
    }
}
