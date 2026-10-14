namespace TE.DomainStorytellingApplied.Domain;

public sealed record BookingPolicy
{
    public BookingPolicy(
        IEnumerable<BookerType> allowedBookerTypes,
        bool requiresReview,
        TimeSpan minDuration,
        TimeSpan maxDuration,
        TimeOnly opensAt,
        TimeOnly closesAt)
    {
        ArgumentNullException.ThrowIfNull(allowedBookerTypes);

        var bookerTypes = allowedBookerTypes.Distinct().Order().ToArray();

        if (bookerTypes.Length == 0)
        {
            throw new ArgumentException("At least one booker type must be allowed.", nameof(allowedBookerTypes));
        }

        if (bookerTypes.Any(type => !Enum.IsDefined(type)))
        {
            throw new ArgumentException("All booker types must be valid.", nameof(allowedBookerTypes));
        }

        if (minDuration <= TimeSpan.Zero)
        {
            throw new ArgumentException("Min duration must be greater than zero.", nameof(minDuration));
        }

        if (minDuration > maxDuration)
        {
            throw new ArgumentException("Min duration cannot be greater than max duration.", nameof(minDuration));
        }

        if (opensAt >= closesAt)
        {
            throw new ArgumentException("Opening time must be before closing time.", nameof(opensAt));
        }

        AllowedBookerTypes = bookerTypes;
        RequiresReview = requiresReview;
        MinDuration = minDuration;
        MaxDuration = maxDuration;
        OpensAt = opensAt;
        ClosesAt = closesAt;
    }

    public IReadOnlyCollection<BookerType> AllowedBookerTypes { get; }

    public bool RequiresReview { get; }

    public TimeSpan MinDuration { get; }

    public TimeSpan MaxDuration { get; }

    public TimeOnly OpensAt { get; }

    public TimeOnly ClosesAt { get; }

    public bool Allows(Booker booker, Timeslot timeslot, DateTimeOffset now)
    {
        ArgumentNullException.ThrowIfNull(booker);
        ArgumentNullException.ThrowIfNull(timeslot);

        return AllowedBookerTypes.Contains(booker.Type)
            && timeslot.Start >= now
            && timeslot.Duration >= MinDuration
            && timeslot.Duration <= MaxDuration
            && IsWithinOpeningHours(timeslot);
    }

    public bool Equals(BookingPolicy? other) =>
        other is not null
        && AllowedBookerTypes.SequenceEqual(other.AllowedBookerTypes)
        && RequiresReview == other.RequiresReview
        && MinDuration == other.MinDuration
        && MaxDuration == other.MaxDuration
        && OpensAt == other.OpensAt
        && ClosesAt == other.ClosesAt;

    public override int GetHashCode()
    {
        var hash = new HashCode();

        foreach (var bookerType in AllowedBookerTypes)
        {
            hash.Add(bookerType);
        }

        hash.Add(RequiresReview);
        hash.Add(MinDuration);
        hash.Add(MaxDuration);
        hash.Add(OpensAt);
        hash.Add(ClosesAt);

        return hash.ToHashCode();
    }

    private bool IsWithinOpeningHours(Timeslot timeslot) =>
        timeslot.Start.Date == timeslot.End.Date
        && TimeOnly.FromDateTime(timeslot.Start.DateTime) >= OpensAt
        && TimeOnly.FromDateTime(timeslot.End.DateTime) <= ClosesAt;
}
