namespace TE.DomainStorytellingApplied.Domain.Tests;

public class PriceListTests
{
    private static Money Sek(decimal amount)
    {
        return new Money(amount, "SEK");
    }

    [Fact]
    public void GivenAssociationPriceInAnotherCurrency_WhenCreating_ShouldThrow()
    {
        Should.Throw<ArgumentException>(() => new PriceList(Sek(400m), new Money(20m, "EUR"), Sek(100m)));
    }

    [Fact]
    public void GivenRegionPriceInAnotherCurrency_WhenCreating_ShouldThrow()
    {
        Should.Throw<ArgumentException>(() => new PriceList(Sek(400m), Sek(200m), new Money(10m, "EUR")));
    }

    [Theory]
    [InlineData(BookerType.Private, 400)]
    [InlineData(BookerType.Association, 200)]
    [InlineData(BookerType.Region, 100)]
    public void GivenABookerType_WhenAskingForPricePerHour_ShouldReturnThatTypesPrice(BookerType bookerType, int expectedAmount)
    {
        var prices = new PriceList(Sek(400m), Sek(200m), Sek(100m));

        prices.PerHourFor(bookerType).ShouldBe(Sek(expectedAmount));
    }

    [Fact]
    public void GivenAnUnknownBookerType_WhenAskingForPricePerHour_ShouldThrow()
    {
        var prices = new PriceList(Sek(400m), Sek(200m), Sek(100m));

        Should.Throw<ArgumentOutOfRangeException>(() => prices.PerHourFor((BookerType)99));
    }
}
