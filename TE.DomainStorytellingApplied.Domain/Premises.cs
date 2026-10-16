namespace TE.DomainStorytellingApplied.Domain;

public class Premises
{
    public Id PremisesId { get; }
    public PremisesName Name { get; }
    public Address Address { get; }
    public Capacity Capacity { get; }
    public IReadOnlyList<string> Equipment { get; }

    public Premises(
        Id premisesId,
        PremisesName name,
        Address address,
        Capacity capacity,
        IEnumerable<string> equipment)
    {
        PremisesId = premisesId;
        Name = name;
        Address = address;
        Capacity = capacity;
        Equipment = equipment.ToArray();
    }
}