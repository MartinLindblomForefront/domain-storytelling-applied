public sealed record PremisesName
{
    public string Value { get; private set; }

    private PremisesName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Premises name must not be empty", nameof(value));

        Value = value;
    }

    public static PremisesName Create(string value) => new(value);
}