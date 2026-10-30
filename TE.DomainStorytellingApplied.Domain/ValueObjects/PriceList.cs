namespace TE.DomainStorytellingApplied.Domain;

/// <summary>
/// A value object: what a venue costs per hour for each type of booker.
/// It is immutable, and because it is a record, two PriceLists with the same prices are equal.
/// </summary>
public sealed record PriceList
{
    // No setters: the values are set once in the constructor, so the rule checked there can never be broken later.
    public Money PrivatePerHour { get; }

    public Money AssociationPerHour { get; }

    public Money RegionPerHour { get; }

    public PriceList(Money privatePerHour, Money associationPerHour, Money regionPerHour)
    {
        if (associationPerHour.Currency != privatePerHour.Currency || regionPerHour.Currency != privatePerHour.Currency)
        {
            throw new ArgumentException("All prices in a price list must be in the same currency.");
        }

        PrivatePerHour = privatePerHour;
        AssociationPerHour = associationPerHour;
        RegionPerHour = regionPerHour;
    }

    public Money PerHourFor(BookerType bookerType)
    {
        return bookerType switch
        {
            BookerType.Private => PrivatePerHour,
            BookerType.Association => AssociationPerHour,
            BookerType.Region => RegionPerHour,
            _ => throw new ArgumentOutOfRangeException(nameof(bookerType), "Unknown booker type.")
        };
    }
}
