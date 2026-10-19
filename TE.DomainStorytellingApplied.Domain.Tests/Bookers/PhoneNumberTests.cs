using TE.DomainStorytellingApplied.Domain.Bookers;

namespace TE.DomainStorytellingApplied.Domain.Tests.Bookers;

public class PhoneNumberTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void GivenEmptyValue_WhenCreatingPhoneNumber_ShouldThrow(string? value)
    {
        Should.Throw<ArgumentException>(() => new PhoneNumber(value!));
    }

    [Fact]
    public void GivenValidValue_WhenCreatingPhoneNumber_ShouldSucceed()
    {
        var phoneNumber = new PhoneNumber("070-1234567");

        phoneNumber.Value.ShouldBe("070-1234567");
    }
}
