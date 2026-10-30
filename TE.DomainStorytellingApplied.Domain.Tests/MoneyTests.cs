namespace TE.DomainStorytellingApplied.Domain.Tests;

public class MoneyTests
{
    [Fact]
    public void GivenNegativeAmount_WhenCreatingMoney_ShouldThrow()
    {
        Should.Throw<DomainException>(() => new Money(-1));
    }
}
