namespace TE.DomainStorytellingApplied.Domain;

public class Booker
{
    public Id BookerId { get; }
    public BookerName Name { get; }

    public Booker(Id bookerId, BookerName name)
    {
        BookerId = bookerId;
        Name = name;
    }
}