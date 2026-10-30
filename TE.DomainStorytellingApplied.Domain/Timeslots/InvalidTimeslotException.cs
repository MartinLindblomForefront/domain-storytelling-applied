namespace TE.DomainStorytellingApplied.Domain.Timeslots;

public sealed class InvalidTimeslotException : DomainException
{
    private InvalidTimeslotException(string message) : base(message) { }

    public static InvalidTimeslotException EndMustBeAfterStart(DateTimeOffset start, DateTimeOffset end) =>
        new($"Timeslot must end after it starts (start: {start:O}, end: {end:O}).");
}
