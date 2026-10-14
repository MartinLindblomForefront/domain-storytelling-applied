namespace TE.DomainStorytellingApplied.Domain.Tests;

public class BookerTests
{
    [Fact]
    public void GivenValidValues_WhenCreatingBooker_ShouldSetProperties()
    {
        var booker = new Booker("Anna Andersson", "anna@example.com", BookerType.PrivateIndividual);

        booker.Id.ShouldNotBe(Guid.Empty);
        booker.Name.ShouldBe("Anna Andersson");
        booker.Email.ShouldBe("anna@example.com");
        booker.Type.ShouldBe(BookerType.PrivateIndividual);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void GivenEmptyName_WhenCreatingBooker_ShouldThrow(string name)
    {
        Should.Throw<ArgumentException>(() => new Booker(name, "anna@example.com", BookerType.PrivateIndividual));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("not-an-email")]
    [InlineData("anna@")]
    public void GivenInvalidEmail_WhenCreatingBooker_ShouldThrow(string email)
    {
        Should.Throw<ArgumentException>(() => new Booker("Anna Andersson", email, BookerType.PrivateIndividual));
    }

    [Fact]
    public void GivenUndefinedBookerType_WhenCreatingBooker_ShouldThrow()
    {
        Should.Throw<ArgumentException>(() => new Booker("Anna Andersson", "anna@example.com", (BookerType)999));
    }
}
