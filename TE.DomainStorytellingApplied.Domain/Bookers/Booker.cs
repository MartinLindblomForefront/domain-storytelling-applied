using TE.DomainStorytellingApplied.Domain.Common;

namespace TE.DomainStorytellingApplied.Domain.Bookers;

public class Booker
{
    public Guid Id { get; private init; }
    public string FirstName { get; private init; }
    public string LastName { get; private init; }
    public string PhoneNumber { get; private init; }
    public string Email { get; private init; }
    public IBookerType Type { get; private init; }

    public Booker(string firstName, string lastName, string phoneNumber, string email, IBookerType type)
    {
        Guard.EnsureNotEmpty(firstName);
        Guard.EnsureNotEmpty(lastName);
        Guard.EnsureNotEmpty(phoneNumber);
        Guard.EnsureNotEmpty(email);
        ArgumentNullException.ThrowIfNull(type);

        Id = Guid.NewGuid();
        FirstName = firstName;
        LastName = lastName;
        PhoneNumber = phoneNumber;
        Email = email;
        Type = type;
    }
}
