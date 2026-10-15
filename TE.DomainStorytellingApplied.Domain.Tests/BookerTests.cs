namespace TE.DomainStorytellingApplied.Domain.Tests;

public class BookerTests
{
    [Fact]
    public void WhenCreatingBookerWithValidParameters_ShouldCreateBooker()
    {
        var id = new Id("b1");
        var booker = new Booker(id, "Anna");

        booker.Id.ShouldBe(id);
        booker.Name.ShouldBe("Anna");
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public void WhenCreatingBookerWithBlankName_ShouldThrowException(string name)
    {
        Should.Throw<ArgumentException>(() => new Booker(new Id("b1"), name));
    }
}
