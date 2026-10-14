namespace TE.DomainStorytellingApplied.Domain.Tests;

public class IdTests
{
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void GivenEmptyValue_WhenCreating_ShouldThrow(string value)
    {
        Should.Throw<Exception>(() => Id.From(value));
    }

    [Fact]
    public void GivenTwoGeneratedIds_WhenComparing_ShouldNotBeEqual()
    {
        var first = Id.Generate();
        var second = Id.Generate();

        first.ShouldNotBe(second);
    }

    [Fact]
    public void GivenSameValue_WhenComparing_ShouldBeEqual()
    {
        var first = Id.From("abc");
        var second = Id.From("abc");

        first.ShouldBe(second);
    }
}
