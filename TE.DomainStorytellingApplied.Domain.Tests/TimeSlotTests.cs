namespace TE.DomainStorytellingApplied.Domain.Tests;

public class TimeSlotTests
{
    [Fact]
    public void GivenMinutesPastTheHour_WhenCreatingSlot_ShouldThrow()
    {
        Should.Throw<DomainException>(() => new TimeSlot(new DateTime(2026, 10, 10, 18, 30, 0)));
    }
}
