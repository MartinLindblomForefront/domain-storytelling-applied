using System.Net.Mail;

namespace TE.DomainStorytellingApplied.Domain;

public sealed class Booker
{
    public Booker(string name, string email, BookerType type)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(email);

        if (!MailAddress.TryCreate(email, out var address) || address.Address != email)
        {
            throw new ArgumentException("Email has an invalid format.", nameof(email));
        }

        if (!Enum.IsDefined(type))
        {
            throw new ArgumentException("Booker type is not valid.", nameof(type));
        }

        Id = Guid.NewGuid();
        Name = name;
        Email = email;
        Type = type;
    }

    public Guid Id { get; }

    public string Name { get; }

    public string Email { get; }

    public BookerType Type { get; }
}
