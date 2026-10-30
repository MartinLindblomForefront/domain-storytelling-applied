namespace TE.DomainStorytellingApplied.Domain.Tests.Premises;

using TE.DomainStorytellingApplied.Domain.Premises;

public class PremiseTests
{
    private static readonly Guid SomeId = Guid.Parse("11111111-1111-1111-1111-111111111111");

    [Fact]
    public void GivenIdAndName_WhenCreatingPremise_ShouldExposeIdAndName()
    {
        var premise = new Premise(SomeId, "Conference Room A");

        premise.Id.ShouldBe(SomeId);
        premise.Name.ShouldBe("Conference Room A");
    }

    [Fact]
    public void GivenNameWithSurroundingWhitespace_WhenCreatingPremise_ShouldTrimName()
    {
        var premise = new Premise(SomeId, "  Conference Room A  ");

        premise.Name.ShouldBe("Conference Room A");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void GivenBlankName_WhenCreatingPremise_ShouldThrowInvalidPremiseException(string name)
    {
        Should.Throw<InvalidPremiseException>(() => new Premise(SomeId, name));
    }

    [Fact]
    public void GivenNullName_WhenCreatingPremise_ShouldThrowInvalidPremiseException()
    {
        Should.Throw<InvalidPremiseException>(() => new Premise(SomeId, null!));
    }

    [Fact]
    public void GivenEmptyId_WhenCreatingPremise_ShouldThrowInvalidPremiseException()
    {
        Should.Throw<InvalidPremiseException>(() => new Premise(Guid.Empty, "Conference Room A"));
    }

    [Fact]
    public void GivenInvalidPremise_WhenCreatingPremise_ShouldThrowDomainException()
    {
        var exception = Should.Throw<InvalidPremiseException>(() => new Premise(Guid.Empty, "Conference Room A"));

        exception.ShouldBeAssignableTo<DomainException>();
        exception.Message.ShouldNotBeNullOrWhiteSpace();
    }

    [Fact]
    public void GivenSameIdAndName_WhenComparing_ShouldBeEqual()
    {
        var first = new Premise(SomeId, "Conference Room A");
        var second = new Premise(SomeId, "Conference Room A");

        first.ShouldBe(second);
    }
}
