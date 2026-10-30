using TE.DomainStorytellingApplied.Domain.Bookers;
using TE.DomainStorytellingApplied.Domain.Timeslots;

namespace TE.DomainStorytellingApplied.Domain.Policies;

// Domain policy
public sealed class BookingPolicy
{
    private static readonly TimeSpan MaxDurationWithoutReview = TimeSpan.FromHours(4);

    public bool RequiresReview(BookerType bookerType, Timeslot timeslot)
    {
        ArgumentNullException.ThrowIfNull(timeslot);

        return bookerType switch
        {
            BookerType.MunicipalityEmployee => false,
            BookerType.AssociationRepresentative => timeslot.Duration > MaxDurationWithoutReview,
            BookerType.PrivateIndividual => true,
            _ => throw new ArgumentOutOfRangeException(nameof(bookerType), bookerType, null),
        };
    }
}
