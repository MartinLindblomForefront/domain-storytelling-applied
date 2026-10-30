using TE.DomainStorytellingApplied.Domain.Aggregates.Bookers.ValueObjects;

namespace TE.DomainStorytellingApplied.Domain.Tests.Aggregates.Bookers.ValueObjects;

public class BookerIdTests
{
    [Fact]
    public void GivenValidGuid_WhenCreatingBookerId_ShouldContainGuid()
    {
        var value = Guid.NewGuid();

        var bookerId = new BookerId(value);

        bookerId.Value.ShouldBe(value);
    }

    [Fact]
    public void GivenEmptyGuid_WhenCreatingBookerId_ShouldThrowArgumentException()
    {
        var exception = Should.Throw<ArgumentException>(() => new BookerId(Guid.Empty));

        exception.Message.ShouldContain("Booker ID cannot be empty.");
        exception.ParamName.ShouldBe("value");
    }

    [Fact]
    public void GivenSameGuid_WhenComparingBookerIds_ShouldBeEqual()
    {
        var value = Guid.NewGuid();
        var first = new BookerId(value);
        var second = new BookerId(value);

        first.ShouldBe(second);
    }

    [Fact]
    public void GivenDifferentGuids_WhenComparingBookerIds_ShouldNotBeEqual()
    {
        var first = new BookerId(Guid.NewGuid());
        var second = new BookerId(Guid.NewGuid());

        first.ShouldNotBe(second);
    }
}
