public sealed record BookerName
{
    public string Value { get; private set; }

    private BookerName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Booker name must not be empty", nameof(value));

        Value = value;
    }

    public static BookerName Create(string value) => new(value);
}