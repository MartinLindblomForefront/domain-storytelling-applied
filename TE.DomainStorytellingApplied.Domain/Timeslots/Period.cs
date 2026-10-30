using TE.DomainStorytellingApplied.Domain.Shared;
using TE.DomainStorytellingApplied.Domain.Timeslots.Rules;

namespace TE.DomainStorytellingApplied.Domain.Timeslots;

public sealed record Period
{
    public Period(DateTime startUtc, DateTime endUtc)
    {
        Rule.Check(new PeriodMustBeInUtc(startUtc, endUtc));
        Rule.Check(new PeriodMustEndAfterItStarts(startUtc, endUtc));

        StartUtc = startUtc;
        EndUtc = endUtc;
    }

    public DateTime StartUtc { get; }

    public DateTime EndUtc { get; }

    public TimeSpan Duration => EndUtc - StartUtc;
}
