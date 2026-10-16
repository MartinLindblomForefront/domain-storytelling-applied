namespace TE.DomainStorytellingApplied.Domain.Tests;

public class BookerNameTests
{
    [Fact]
    public void GivenValidName_WhenCreating_ShouldKeepValue()
    {
        var name = BookerName.Create("Anna Andersson");

        name.Value.ShouldBe("Anna Andersson");
    }

    [Fact]
    public void GivenEmptyName_WhenCreating_ShouldThrow()
    {
        Should.Throw<ArgumentException>(() => BookerName.Create(""));
    }

    [Fact]
    public void GivenWhitespaceName_WhenCreating_ShouldThrow()
    {
        Should.Throw<ArgumentException>(() => BookerName.Create(" "));
    }
}