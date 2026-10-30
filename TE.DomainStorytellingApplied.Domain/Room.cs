namespace TE.DomainStorytellingApplied.Domain;

public class Room
{
    public Room(int id, string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        Id = id;
        Name = name;
    }

    public int Id { get; }
    public string Name { get; }
}
