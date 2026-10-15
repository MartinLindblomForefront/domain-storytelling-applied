namespace TE.DomainStorytellingApplied.Domain.Tests.Fakes;

public sealed class InMemoryTimeslotRepository : ITimeslotRepository
{
    private readonly List<Timeslot> _timeslots = new();

    public void Add(Timeslot timeslot) => _timeslots.Add(timeslot);

    public Timeslot? Find(Id timeslotId) => _timeslots.FirstOrDefault(t => t.Id == timeslotId);

    public IReadOnlyList<Timeslot> FindByPremises(Id premisesId) =>
        _timeslots.Where(t => t.PremisesId == premisesId).ToList();
}
