namespace TE.DomainStorytellingApplied.Domain.Tests;

public class MoneyTests
{
    [Fact]
    public void GivenNegativeAmount_WhenCreating_ShouldThrow()
    {
        Should.Throw<ArgumentException>(() => new Money(-1m, "SEK"));
    }

    [Fact]
    public void GivenZeroAmount_WhenCreating_ShouldBeAllowed()
    {
        var money = new Money(0m, "SEK");

        money.Amount.ShouldBe(0m);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void GivenEmptyCurrency_WhenCreating_ShouldThrow(string currency)
    {
        Should.Throw<ArgumentException>(() => new Money(100m, currency));
    }

    // The one example of equality by value. The other value objects are records too, and behave the same.
    [Fact]
    public void GivenTwoMoneyWithSameAmountAndCurrency_WhenComparing_ShouldBeEqual()
    {
        var first = new Money(100m, "SEK");
        var second = new Money(100m, "SEK");

        first.ShouldBe(second);
    }

    [Fact]
    public void GivenAFactor_WhenMultiplying_ShouldMultiplyTheAmountAndKeepTheCurrency()
    {
        var money = new Money(100m, "SEK");

        var result = money.Multiply(1.5m);

        result.ShouldBe(new Money(150m, "SEK"));
    }

    [Fact]
    public void GivenAResultWithManyDecimals_WhenMultiplying_ShouldRoundToTwoDecimals()
    {
        var pricePerHour = new Money(140m, "SEK");
        var fiftyMinutesInHours = 50m / 60m;

        var result = pricePerHour.Multiply(fiftyMinutesInHours);

        // 140 * 0.8333... = 116.666..., which is rounded to 116.67.
        result.ShouldBe(new Money(116.67m, "SEK"));
    }

    [Fact]
    public void GivenAResultExactlyHalfwayBetweenTwoDecimals_WhenMultiplying_ShouldRoundUp()
    {
        var money = new Money(0.25m, "SEK");

        var result = money.Multiply(0.5m);

        // 0.25 * 0.5 = 0.125, exactly halfway between 0.12 and 0.13.
        result.ShouldBe(new Money(0.13m, "SEK"));
    }

    [Fact]
    public void GivenMoney_WhenMultiplying_ShouldNotChangeTheOriginal()
    {
        var money = new Money(100m, "SEK");

        money.Multiply(2m);

        money.Amount.ShouldBe(100m);
    }

    [Fact]
    public void GivenNegativeFactor_WhenMultiplying_ShouldThrow()
    {
        var money = new Money(100m, "SEK");

        Should.Throw<ArgumentException>(() => money.Multiply(-1m));
    }
}
