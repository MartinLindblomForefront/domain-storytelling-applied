namespace TE.DomainStorytellingApplied.Domain.Tests;

public class AddressTests
{
    [Fact]
    public void GivenValidAddress_WhenCreating_ShouldKeepValue()
    {
        var address = Address.Create("Main Street 1");

        address.Value.ShouldBe("Main Street 1");
    }

    [Fact]
    public void GivenEmptyAddress_WhenCreating_ShouldThrow()
    {
        Should.Throw<ArgumentException>(() => Address.Create(""));
    }

    [Fact]
    public void GivenWhitespaceAddress_WhenCreating_ShouldThrow()
    {
        Should.Throw<ArgumentException>(() => Address.Create(" "));
    }
}