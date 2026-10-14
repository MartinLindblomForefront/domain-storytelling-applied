namespace TE.DomainStorytellingApplied.Domain;

public interface IBookingRepository
{
    void Add(Booking booking);

    Booking? GetById(Id id);
}
