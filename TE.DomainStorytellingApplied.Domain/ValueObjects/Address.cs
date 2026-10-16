public sealed record Address
{
    public string Value { get; private set; }

    private Address(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Address must not be empty", nameof(value));

        Value = value;
    }

    public static Address Create(string value) => new(value);
}