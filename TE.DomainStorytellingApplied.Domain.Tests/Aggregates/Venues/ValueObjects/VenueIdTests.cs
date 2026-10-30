using TE.DomainStorytellingApplied.Domain.Aggregates.Venues.ValueObjects;

namespace TE.DomainStorytellingApplied.Domain.Tests.Aggregates.Venues.ValueObjects;

public class VenueIdTests
{
    [Fact]
    public void GivenValidGuid_WhenCreatingVenueId_ShouldContainGuid()
    {
        var value = Guid.NewGuid();

        var venueId = new VenueId(value);

        venueId.Value.ShouldBe(value);
    }

    [Fact]
    public void GivenEmptyGuid_WhenCreatingVenueId_ShouldThrowArgumentException()
    {
        var exception = Should.Throw<ArgumentException>(() => new VenueId(Guid.Empty));

        exception.Message.ShouldContain("Venue ID cannot be empty.");
        exception.ParamName.ShouldBe("value");
    }

    [Fact]
    public void GivenSameGuid_WhenComparingVenueIds_ShouldBeEqual()
    {
        var value = Guid.NewGuid();
        var first = new VenueId(value);
        var second = new VenueId(value);

        first.ShouldBe(second);
    }

    [Fact]
    public void GivenDifferentGuids_WhenComparingVenueIds_ShouldNotBeEqual()
    {
        var first = new VenueId(Guid.NewGuid());
        var second = new VenueId(Guid.NewGuid());

        first.ShouldNotBe(second);
    }
}
