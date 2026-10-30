namespace TE.DomainStorytellingApplied.Domain;

public class Booker
{
    public Booker(int id, string name, string email)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(email);

        Id = id;
        Name = name;
        Email = email;
    }

    public int Id { get; }
    public string Name { get; }
    public string Email { get; }
}
