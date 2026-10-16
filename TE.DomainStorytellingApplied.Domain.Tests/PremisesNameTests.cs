namespace TE.DomainStorytellingApplied.Domain.Tests;

public class PremisesNameTests
{
    [Fact]
    public void GivenValidName_WhenCreating_ShouldKeepValue()
    {
        var name = PremisesName.Create("Conference Room");

        name.Value.ShouldBe("Conference Room");
    }

    [Fact]
    public void GivenEmptyName_WhenCreating_ShouldThrow()
    {
        Should.Throw<ArgumentException>(() => PremisesName.Create(""));
    }

    [Fact]
    public void GivenWhitespaceName_WhenCreating_ShouldThrow()
    {
        Should.Throw<ArgumentException>(() => PremisesName.Create(" "));
    }
}