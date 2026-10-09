namespace TE.DomainStorytellingApplied.Domain;

public sealed class Premises
{
    private readonly List<Timeslot> _timeslots;
    private readonly HashSet<Timeslot> _reservedTimeslots = [];

    public Id Id { get; }

    public string Name { get; }

    public decimal Price { get; }

    public IReadOnlyList<Timeslot> Timeslots => _timeslots;

    public Premises(Id id, string name, decimal price, IEnumerable<Timeslot> timeslots)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new Exception("Premises must have a name");

        if (price < 0)
            throw new Exception("Price may not be negative");

        Id = id;
        Name = name;
        Price = price;
        _timeslots = timeslots.Distinct().ToList();
    }

    public bool IsReserved(Timeslot timeslot) => _reservedTimeslots.Contains(timeslot);

    public void Reserve(Timeslot timeslot)
    {
        if (!_timeslots.Contains(timeslot))
            throw new Exception("Timeslot is not offered by this premises");

        if (!_reservedTimeslots.Add(timeslot))
            throw new Exception("Timeslot is already reserved");
    }

    public void Release(Timeslot timeslot)
    {
        if (!_reservedTimeslots.Remove(timeslot))
            throw new Exception("Timeslot is not reserved");
    }
}
