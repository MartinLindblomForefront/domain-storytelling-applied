using TE.DomainStorytellingApplied.Domain.Aggregates.Venues.AggregateRoots;
using TE.DomainStorytellingApplied.Domain.Aggregates.Venues.ValueObjects;

namespace TE.DomainStorytellingApplied.Domain.Tests.Aggregates.Venues.AggregateRoots;

public class VenueTests
{
    [Fact]
    public void GivenValidDetails_WhenCreatingVenue_ShouldCreateVenue()
    {
        var id = new VenueId(Guid.NewGuid());

        var venue = new Venue(id, "Large hall", 100);

        venue.Id.ShouldBe(id);
        venue.Name.ShouldBe("Large hall");
        venue.Capacity.ShouldBe(100);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void GivenEmptyName_WhenCreatingVenue_ShouldThrowArgumentException(string name)
    {
        var id = new VenueId(Guid.NewGuid());

        var exception = Should.Throw<ArgumentException>(() => new Venue(id, name, 100));

        exception.Message.ShouldContain("Venue name cannot be empty.");
        exception.ParamName.ShouldBe("name");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void GivenNonPositiveCapacity_WhenCreatingVenue_ShouldThrowArgumentOutOfRangeException(
        int capacity)
    {
        var id = new VenueId(Guid.NewGuid());

        var exception = Should.Throw<ArgumentOutOfRangeException>(
            () => new Venue(id, "Large hall", capacity));

        exception.Message.ShouldContain("Venue capacity must be greater than zero.");
        exception.ParamName.ShouldBe("capacity");
    }

    [Fact]
    public void GivenSameVenueId_WhenComparingVenues_ShouldBeEqual()
    {
        var id = new VenueId(Guid.NewGuid());
        var first = new Venue(id, "Large hall", 100);
        var second = new Venue(id, "Small hall", 20);

        first.ShouldBe(second);
    }

    [Fact]
    public void GivenDifferentVenueIds_WhenComparingVenues_ShouldNotBeEqual()
    {
        var first = new Venue(new VenueId(Guid.NewGuid()), "Large hall", 100);
        var second = new Venue(new VenueId(Guid.NewGuid()), "Large hall", 100);

        first.ShouldNotBe(second);
    }
}
