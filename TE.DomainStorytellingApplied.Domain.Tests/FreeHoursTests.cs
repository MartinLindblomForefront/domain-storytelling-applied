using static TE.DomainStorytellingApplied.Domain.Tests.TestData;

namespace TE.DomainStorytellingApplied.Domain.Tests;

public class FreeHoursTests
{
    private static readonly DateTime Tomorrow = Tomorrow18.Start.Date;

    [Fact]
    public void GivenEmptyVenue_WhenListingFreeHours_ShouldReturnAllOpeningHours()
    {
        NewVenue().FreeHours(Tomorrow, Now).Count.ShouldBe(14);
    }

    [Fact]
    public void GivenBookedHour_WhenListingFreeHours_ShouldExcludeIt()
    {
        var venue = NewVenue();
        venue.Reserve(Tomorrow18, BookerType.Association, Now);

        var free = venue.FreeHours(Tomorrow, Now);

        free.Count.ShouldBe(13);
        free.ShouldNotContain(Tomorrow18);
    }
}
