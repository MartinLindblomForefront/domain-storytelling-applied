using TE.DomainStorytellingApplied.Domain.Bookers;

namespace TE.DomainStorytellingApplied.Domain.Tests.Bookers;

public class BookerTests
{
    private const string FirstName = "Anna";
    private const string LastName = "Andersson";
    private const string PhoneNumber = "070-1234567";
    private const string Email = "anna@example.com";
    private static readonly IBookerType DefaultType = new PrivateIndividual();

    public static TheoryData<IBookerType> AllBookerTypes =>
    [
        new MunicipalityEmployee(),
        new PrivateIndividual(),
        new AssociationRepresentative("IFK Göteborg")
    ];

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void GivenEmptyFirstName_WhenCreatingBooker_ShouldThrow(string? firstName)
    {
        Should.Throw<ArgumentException>(() =>
            new Booker(firstName!, LastName, PhoneNumber, Email, DefaultType));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void GivenEmptyLastName_WhenCreatingBooker_ShouldThrow(string? lastName)
    {
        Should.Throw<ArgumentException>(() =>
            new Booker(FirstName, lastName!, PhoneNumber, Email, DefaultType));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void GivenEmptyPhoneNumber_WhenCreatingBooker_ShouldThrow(string? phoneNumber)
    {
        Should.Throw<ArgumentException>(() =>
            new Booker(FirstName, LastName, phoneNumber!, Email, DefaultType));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void GivenEmptyEmail_WhenCreatingBooker_ShouldThrow(string? email)
    {
        Should.Throw<ArgumentException>(() =>
            new Booker(FirstName, LastName, PhoneNumber, email!, DefaultType));
    }

    [Fact]
    public void GivenNullBookerType_WhenCreatingBooker_ShouldThrow()
    {
        Should.Throw<ArgumentNullException>(() =>
            new Booker(FirstName, LastName, PhoneNumber, Email, null!));
    }

    [Fact]
    public void GivenAssociationRepresentativeWithoutOrganization_WhenCreatingBooker_ShouldThrow()
    {
        Should.Throw<ArgumentException>(() =>
            new Booker(FirstName, LastName, PhoneNumber, Email, new AssociationRepresentative("")));
    }

    [Theory]
    [MemberData(nameof(AllBookerTypes))]
    public void GivenValidValues_WhenCreatingBooker_ShouldSucceed(IBookerType type)
    {
        var booker = new Booker(FirstName, LastName, PhoneNumber, Email, type);

        booker.Id.ShouldNotBe(Guid.Empty);
        booker.FirstName.ShouldBe(FirstName);
        booker.LastName.ShouldBe(LastName);
        booker.PhoneNumber.ShouldBe(PhoneNumber);
        booker.Email.ShouldBe(Email);
        booker.Type.ShouldBe(type);
    }

    [Fact]
    public void GivenTwoBookers_WhenCreated_ShouldHaveUniqueIds()
    {
        var first = new Booker(FirstName, LastName, PhoneNumber, Email, DefaultType);
        var second = new Booker(FirstName, LastName, PhoneNumber, Email, DefaultType);

        first.Id.ShouldNotBe(second.Id);
    }
}
