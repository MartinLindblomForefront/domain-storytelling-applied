namespace TE.DomainStorytellingApplied.Domain.Shared;

public sealed class DomainValidationException(string domain, string message) : Exception(message)
{
    public string Domain { get; } = domain;
}
