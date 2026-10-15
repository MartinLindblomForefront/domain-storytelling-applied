namespace TE.DomainStorytellingApplied.Domain.Tests;

public class PremisesTests
{
    [Fact]
    public void WhenCreatingPremisesWithValidParameters_ShouldCreatePremises()
    {
        var id = new Id("p1");
        var premises = new Premises(id, "Hall");

        premises.Id.ShouldBe(id);
        premises.Name.ShouldBe("Hall");
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public void WhenCreatingPremisesWithBlankName_ShouldThrowException(string name)
    {
        Should.Throw<ArgumentException>(() => new Premises(new Id("p1"), name));
    }
}
