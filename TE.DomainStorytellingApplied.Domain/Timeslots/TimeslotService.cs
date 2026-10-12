namespace TE.DomainStorytellingApplied.Domain;

public sealed class TimeslotService(ITimeslotRepository timeslots)
{
    public Timeslot Create(Premises premises, Id id, DateTimeOffset start, DateTimeOffset end)
    {
        if (timeslots.Find(id) is not null)
        {
            throw new InvalidOperationException($"Timeslot '{id}' already exists.");
        }

        var timeslot = new Timeslot(id, premises.Id, start, end);

        if (timeslots.FindByPremises(premises.Id).Any(t => t.Overlaps(timeslot)))
        {
            throw new InvalidOperationException("Timeslot overlaps an existing timeslot.");
        }

        timeslots.Add(timeslot);
        return timeslot;
    }
}
