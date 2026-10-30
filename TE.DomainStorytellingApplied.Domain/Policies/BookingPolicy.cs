using TE.DomainStorytellingApplied.Domain.Policies.Rules;
using TE.DomainStorytellingApplied.Domain.Shared;
using TE.DomainStorytellingApplied.Domain.Timeslots;
using TE.DomainStorytellingApplied.Domain.Timeslots.Rules;

namespace TE.DomainStorytellingApplied.Domain.Policies;

public sealed class BookingPolicy(TimeSpan reviewThreshold)
{
    public static readonly TimeSpan MinimumDuration = TimeSpan.FromHours(1);
    public static readonly TimeSpan MaximumAdvance = TimeSpan.FromDays(365);

    public void Check(IReadOnlyCollection<Timeslot> timeslots, DateTime nowUtc)
    {
        Rule.Check(new TimeslotsMustNotBeEmpty(timeslots));
        Rule.Check(new TimeslotsMustBeInSamePremises(timeslots));
        foreach (var timeslot in timeslots)
            Rule.Check(new TimeslotMustBeAvailable(timeslot));
        Rule.Check(new TotalDurationMustBeAtLeast(TotalDuration(timeslots), MinimumDuration));
        Rule.Check(new ReservationMustNotStartInThePast(timeslots, nowUtc));
        Rule.Check(new ReservationMustNotBeTooFarAhead(timeslots, nowUtc, MaximumAdvance));
    }

    public bool RequiresReview(IReadOnlyCollection<Timeslot> timeslots) =>
        TotalDuration(timeslots) > reviewThreshold;

    private static TimeSpan TotalDuration(IEnumerable<Timeslot> timeslots) =>
        timeslots.Aggregate(TimeSpan.Zero, (sum, timeslot) => sum + timeslot.Period.Duration);
}
