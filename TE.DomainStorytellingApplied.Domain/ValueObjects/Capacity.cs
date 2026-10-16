public sealed record Capacity
{
    public int Value { get; private set; }

    private Capacity(int value)
    {
        if (value <= 0)
            throw new ArgumentOutOfRangeException(nameof(value), "Capacity must be greater than zero");

        Value = value;
    }

    public static Capacity Create(int value) => new(value);
}