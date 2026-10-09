namespace TE.DomainStorytellingApplied.Domain;

public sealed record Timeslot
{
    public DateTimeOffset StartTime { get; private init; }

    public DateTimeOffset EndTime { get; private init; }

    public Timeslot(DateTimeOffset startTime, DateTimeOffset endTime)
    {
        if (endTime <= startTime)
            throw new Exception("Timeslot must end after its start time");

        StartTime = startTime;
        EndTime = endTime;
    }
}
