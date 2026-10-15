namespace TE.DomainStorytellingApplied.Domain;

public interface ITimeslotRepository
{
    void Add(Timeslot timeslot);

    Timeslot? Find(Id timeslotId);

    IReadOnlyList<Timeslot> FindByPremises(Id premisesId);
}
