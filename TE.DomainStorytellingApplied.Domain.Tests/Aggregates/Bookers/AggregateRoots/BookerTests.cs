using TE.DomainStorytellingApplied.Domain.Aggregates.Bookers.AggregateRoots;
using TE.DomainStorytellingApplied.Domain.Aggregates.Bookers.ValueObjects;

namespace TE.DomainStorytellingApplied.Domain.Tests.Aggregates.Bookers.AggregateRoots;

public class BookerTests
{
    [Fact]
    public void GivenValidDetails_WhenCreatingBooker_ShouldCreateBooker()
    {
        var id = new BookerId(Guid.NewGuid());

        var booker = new Booker(id, "Anna Andersson");

        booker.Id.ShouldBe(id);
        booker.Name.ShouldBe("Anna Andersson");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void GivenEmptyName_WhenCreatingBooker_ShouldThrowArgumentException(string name)
    {
        var id = new BookerId(Guid.NewGuid());

        var exception = Should.Throw<ArgumentException>(() => new Booker(id, name));

        exception.Message.ShouldContain("Booker name cannot be empty.");
        exception.ParamName.ShouldBe("name");
    }

    [Fact]
    public void GivenSameBookerId_WhenComparingBookers_ShouldBeEqual()
    {
        var id = new BookerId(Guid.NewGuid());
        var first = new Booker(id, "Anna Andersson");
        var second = new Booker(id, "Anna Karlsson");

        first.ShouldBe(second);
    }

    [Fact]
    public void GivenDifferentBookerIds_WhenComparingBookers_ShouldNotBeEqual()
    {
        var first = new Booker(new BookerId(Guid.NewGuid()), "Anna Andersson");
        var second = new Booker(new BookerId(Guid.NewGuid()), "Anna Andersson");

        first.ShouldNotBe(second);
    }
}
