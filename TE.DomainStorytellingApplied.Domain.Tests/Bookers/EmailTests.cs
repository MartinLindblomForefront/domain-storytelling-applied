using TE.DomainStorytellingApplied.Domain.Bookers;

namespace TE.DomainStorytellingApplied.Domain.Tests.Bookers;

public class EmailTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void GivenEmptyValue_WhenCreatingEmail_ShouldThrow(string? value)
    {
        Should.Throw<ArgumentException>(() => new Email(value!));
    }

    [Fact]
    public void GivenValidValue_WhenCreatingEmail_ShouldSucceed()
    {
        var email = new Email("anna@example.com");

        email.Value.ShouldBe("anna@example.com");
    }
}
