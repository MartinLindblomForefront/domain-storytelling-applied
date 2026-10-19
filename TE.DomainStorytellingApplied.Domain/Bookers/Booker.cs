namespace TE.DomainStorytellingApplied.Domain.Bookers;

public class Booker
{
    public Guid Id { get; private init; }
    public Name FirstName { get; private init; }
    public Name LastName { get; private init; }
    public PhoneNumber PhoneNumber { get; private init; }
    public Email Email { get; private init; }
    public IBookerType Type { get; private init; }

    public Booker(Name firstName, Name lastName, PhoneNumber phoneNumber, Email email, IBookerType type)
    {
        ArgumentNullException.ThrowIfNull(firstName);
        ArgumentNullException.ThrowIfNull(lastName);
        ArgumentNullException.ThrowIfNull(phoneNumber);
        ArgumentNullException.ThrowIfNull(email);
        ArgumentNullException.ThrowIfNull(type);

        Id = Guid.NewGuid();
        FirstName = firstName;
        LastName = lastName;
        PhoneNumber = phoneNumber;
        Email = email;
        Type = type;
    }
}
