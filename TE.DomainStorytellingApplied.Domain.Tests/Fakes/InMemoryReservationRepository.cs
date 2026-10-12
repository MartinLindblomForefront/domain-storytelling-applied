namespace TE.DomainStorytellingApplied.Domain.Tests.Fakes;

public sealed class InMemoryReservationRepository : IReservationRepository
{
    private readonly List<Reservation> _reservations = new();

    public IReadOnlyList<Reservation> All => _reservations;

    public void Add(Reservation reservation) => _reservations.Add(reservation);

    public Reservation? Find(Id reservationId) => _reservations.FirstOrDefault(r => r.Id == reservationId);

    public int CountActiveFor(Id bookerId) => _reservations.Count(r => r.BookerId == bookerId && r.IsActive);
}
