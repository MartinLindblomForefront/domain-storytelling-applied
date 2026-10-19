using TE.DomainStorytellingApplied.Domain.Bookers;

namespace TE.DomainStorytellingApplied.Domain.Tests.Bookers;

public class NameTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void GivenEmptyValue_WhenCreatingName_ShouldThrow(string? value)
    {
        Should.Throw<ArgumentException>(() => new Name(value!));
    }

    [Fact]
    public void GivenValidValue_WhenCreatingName_ShouldSucceed()
    {
        var name = new Name("Anna");

        name.Value.ShouldBe("Anna");
    }
}
