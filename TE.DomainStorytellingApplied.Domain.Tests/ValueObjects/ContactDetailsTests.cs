namespace TE.DomainStorytellingApplied.Domain.Tests;

public class ContactDetailsTests
{
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void GivenEmptyName_WhenCreating_ShouldThrow(string name)
    {
        Should.Throw<ArgumentException>(() => new ContactDetails(name, "anna@example.com", "070-000 00 00"));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("anna.example.com")]
    public void GivenEmptyEmailOrEmailWithoutAtSign_WhenCreating_ShouldThrow(string email)
    {
        Should.Throw<ArgumentException>(() => new ContactDetails("Anna Andersson", email, "070-000 00 00"));
    }
}
