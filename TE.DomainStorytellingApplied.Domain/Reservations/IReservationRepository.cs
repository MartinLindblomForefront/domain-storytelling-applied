namespace TE.DomainStorytellingApplied.Domain;

public interface IReservationRepository
{
    void Add(Reservation reservation);

    Reservation? Find(Id reservationId);

    int CountActiveFor(Id bookerId);
}
