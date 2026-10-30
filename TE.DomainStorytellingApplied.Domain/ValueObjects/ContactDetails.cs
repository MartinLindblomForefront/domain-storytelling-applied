namespace TE.DomainStorytellingApplied.Domain;

/// <summary>
/// A value object: who to contact about a booking.
/// It is immutable, and because it is a record, two ContactDetails with the same values are equal.
/// </summary>
public sealed record ContactDetails
{
    // No setters: the values are set once in the constructor, so the rules checked there can never be broken later.
    public string Name { get; }

    public string Email { get; }

    public string Phone { get; }

    public ContactDetails(string name, string email, string phone)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Name is required.", nameof(name));
        }

        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
        {
            throw new ArgumentException("A valid email address is required.", nameof(email));
        }

        Name = name;
        Email = email;
        Phone = phone;
    }
}
