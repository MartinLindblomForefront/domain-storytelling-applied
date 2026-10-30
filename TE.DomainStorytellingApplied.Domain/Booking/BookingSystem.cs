using TE.DomainStorytellingApplied.Domain.Bookers;
using TE.DomainStorytellingApplied.Domain.Premises;
using TE.DomainStorytellingApplied.Domain.Reservations;
using TE.DomainStorytellingApplied.Domain.Timeslots;

namespace TE.DomainStorytellingApplied.Domain.Booking;

public class BookingSystem
{
    private readonly List<Premise> _premises = [];
    private readonly List<Booker> _bookers = [];
    private readonly List<Timeslot> _timeslots = [];
    private readonly List<Reservation> _reservations = [];

    public IReadOnlyList<Premise> Premises => _premises;
    public IReadOnlyList<Booker> Bookers => _bookers;
    public IReadOnlyList<Timeslot> Timeslots => _timeslots;
    public IReadOnlyList<Reservation> Reservations => _reservations;

    public Premise AddPremise(string name)
    {
        var premise = new Premise(name);
        _premises.Add(premise);
        return premise;
    }

    public Booker AddBooker(string firstName, string lastName, string phoneNumber, string email, IBookerType type)
    {
        var booker = new Booker(firstName, lastName, phoneNumber, email, type);
        _bookers.Add(booker);
        return booker;
    }

    public Timeslot AddTimeslot(Guid premiseId, DateTime startTime, DateTime endTime)
    {
        var premise = FindPremise(premiseId);
        var timeslot = new Timeslot(premise.Id, startTime, endTime);
        _timeslots.Add(timeslot);
        return timeslot;
    }

    public Reservation Reserve(Guid bookerId, Guid timeslotId)
    {
        var booker = _bookers.FirstOrDefault(b => b.Id == bookerId)
            ?? throw new ArgumentException($"No booker with id {bookerId}.", nameof(bookerId));
        var timeslot = _timeslots.FirstOrDefault(t => t.Id == timeslotId)
            ?? throw new ArgumentException($"No timeslot with id {timeslotId}.", nameof(timeslotId));
        var premise = FindPremise(timeslot.PremisesId);

        var reservation = new Reservation(booker, timeslot, premise);
        _reservations.Add(reservation);
        return reservation;
    }

    private Premise FindPremise(Guid premiseId) =>
        _premises.FirstOrDefault(p => p.Id == premiseId)
            ?? throw new ArgumentException($"No premise with id {premiseId}.", nameof(premiseId));
}
