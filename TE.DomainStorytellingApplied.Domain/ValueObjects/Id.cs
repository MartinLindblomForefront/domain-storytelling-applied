public sealed record Id
{
    public string Value { get; private set; }

    private Id(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new Exception("Id must not be empty");

        Value = value;
    }

    public static Id Generate() => new(Guid.NewGuid().ToString());
}