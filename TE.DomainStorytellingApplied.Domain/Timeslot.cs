namespace TE.DomainStorytellingApplied.Domain;

public sealed record Timeslot
{
    public Id Id { get; init; }
    public Id PremisesId { get; init; }
    public DateTimeOffset StartTime { get; init; }
    public DateTimeOffset EndTime { get; init; }

    public Timeslot(Id id, Id premisesId, DateTimeOffset startTime, DateTimeOffset endTime)
    {
        if (endTime <= startTime)
            throw new Exception("Timeslot must end after its start time");

        Id = id;
        PremisesId = premisesId;
        StartTime = startTime;
        EndTime = endTime;
    }
}