using TE.DomainStorytellingApplied.Domain.Shared;
using TE.DomainStorytellingApplied.Domain.Timeslots;

namespace TE.DomainStorytellingApplied.Domain.Tests.Timeslots;

public class PeriodTests
{
    private static DateTime Utc(int hour) => new(2026, 11, 2, hour, 0, 0, DateTimeKind.Utc);

    [Test]
    public void A_period_must_end_after_it_starts()
    {
        // Arrange
        var start = Utc(18);
        var end = Utc(17);

        // Act
        var create = () => new Period(start, end);

        // Assert
        create.Should().Throw<DomainValidationException>().Which.Domain.Should().Be(nameof(Period));
    }

    [Test]
    public void A_period_must_be_in_utc()
    {
        // Arrange
        var localStart = new DateTime(2026, 11, 2, 17, 0, 0, DateTimeKind.Local);

        // Act
        var create = () => new Period(localStart, Utc(18));

        // Assert
        create.Should().Throw<DomainValidationException>().Which.Domain.Should().Be(nameof(Period));
    }
}
