using TE.DomainStorytellingApplied.Domain.Bookers;

namespace TE.DomainStorytellingApplied.Domain.Tests.Bookers;

public class BookerTests
{
    private static readonly Name FirstName = new("Anna");
    private static readonly Name LastName = new("Andersson");
    private static readonly PhoneNumber PhoneNumber = new("070-1234567");
    private static readonly Email Email = new("anna@example.com");
    private static readonly IBookerType DefaultType = new PrivateIndividual();

    public static TheoryData<IBookerType> AllBookerTypes =>
    [
        new MunicipalityEmployee(),
        new PrivateIndividual(),
        new AssociationRepresentative(new Name("IFK Göteborg"))
    ];

    [Fact]
    public void GivenNullFirstName_WhenCreatingBooker_ShouldThrow()
    {
        Should.Throw<ArgumentNullException>(() =>
            new Booker(null!, LastName, PhoneNumber, Email, DefaultType));
    }

    [Fact]
    public void GivenNullLastName_WhenCreatingBooker_ShouldThrow()
    {
        Should.Throw<ArgumentNullException>(() =>
            new Booker(FirstName, null!, PhoneNumber, Email, DefaultType));
    }

    [Fact]
    public void GivenNullPhoneNumber_WhenCreatingBooker_ShouldThrow()
    {
        Should.Throw<ArgumentNullException>(() =>
            new Booker(FirstName, LastName, null!, Email, DefaultType));
    }

    [Fact]
    public void GivenNullEmail_WhenCreatingBooker_ShouldThrow()
    {
        Should.Throw<ArgumentNullException>(() =>
            new Booker(FirstName, LastName, PhoneNumber, null!, DefaultType));
    }

    [Fact]
    public void GivenNullBookerType_WhenCreatingBooker_ShouldThrow()
    {
        Should.Throw<ArgumentNullException>(() =>
            new Booker(FirstName, LastName, PhoneNumber, Email, null!));
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
