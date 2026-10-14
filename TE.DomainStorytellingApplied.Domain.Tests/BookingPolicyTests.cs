namespace TE.DomainStorytellingApplied.Domain.Tests;

public class BookingPolicyTests
{
    private static readonly DateTimeOffset Now = new(2030, 1, 1, 8, 0, 0, TimeSpan.Zero);
    private static readonly TimeOnly OpensAt = new(8, 0);
    private static readonly TimeOnly ClosesAt = new(22, 0);

    private static readonly Booker PrivateBooker =
        new("Anna Andersson", "anna@example.com", BookerType.PrivateIndividual);

    private static readonly Booker AssociationBooker =
        new("Cecilia Carlsson", "cecilia@example.com", BookerType.AssociationRepresentative);

    private static BookingPolicy CreatePolicy(
        BookerType[]? allowedBookerTypes = null,
        bool requiresReview = false) =>
        new(
            allowedBookerTypes ?? [BookerType.PrivateIndividual],
            requiresReview,
            minDuration: TimeSpan.FromHours(1),
            maxDuration: TimeSpan.FromHours(4),
            opensAt: OpensAt,
            closesAt: ClosesAt);

    private static Timeslot Slot(int startHour, int endHour) =>
        new(new DateTimeOffset(2030, 1, 2, startHour, 0, 0, TimeSpan.Zero), new DateTimeOffset(2030, 1, 2, endHour, 0, 0, TimeSpan.Zero));

    [Fact]
    public void GivenValidValues_WhenCreatingPolicy_ShouldSetProperties()
    {
        var policy = CreatePolicy(requiresReview: true);

        policy.AllowedBookerTypes.ShouldBe([BookerType.PrivateIndividual]);
        policy.RequiresReview.ShouldBeTrue();
        policy.MinDuration.ShouldBe(TimeSpan.FromHours(1));
        policy.MaxDuration.ShouldBe(TimeSpan.FromHours(4));
        policy.OpensAt.ShouldBe(OpensAt);
        policy.ClosesAt.ShouldBe(ClosesAt);
    }

    [Fact]
    public void GivenPoliciesWithSameValues_WhenComparing_ShouldBeEqual()
    {
        var first = CreatePolicy(allowedBookerTypes: [BookerType.PrivateIndividual, BookerType.MunicipalityEmployee]);
        var second = CreatePolicy(allowedBookerTypes: [BookerType.MunicipalityEmployee, BookerType.PrivateIndividual]);

        first.ShouldBe(second);
        first.GetHashCode().ShouldBe(second.GetHashCode());
    }

    [Fact]
    public void GivenPoliciesWithDifferentValues_WhenComparing_ShouldNotBeEqual()
    {
        CreatePolicy(requiresReview: true).ShouldNotBe(CreatePolicy(requiresReview: false));
        CreatePolicy(allowedBookerTypes: [BookerType.PrivateIndividual])
            .ShouldNotBe(CreatePolicy(allowedBookerTypes: [BookerType.MunicipalityEmployee]));
    }

    [Fact]
    public void GivenMinDurationGreaterThanMaxDuration_WhenCreatingPolicy_ShouldThrow()
    {
        Should.Throw<ArgumentException>(() => new BookingPolicy(
            [BookerType.PrivateIndividual],
            requiresReview: false,
            minDuration: TimeSpan.FromHours(5),
            maxDuration: TimeSpan.FromHours(4),
            opensAt: OpensAt,
            closesAt: ClosesAt));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-60)]
    public void GivenMinDurationNotGreaterThanZero_WhenCreatingPolicy_ShouldThrow(int minutes)
    {
        Should.Throw<ArgumentException>(() => new BookingPolicy(
            [BookerType.PrivateIndividual],
            requiresReview: false,
            minDuration: TimeSpan.FromMinutes(minutes),
            maxDuration: TimeSpan.FromHours(4),
            opensAt: OpensAt,
            closesAt: ClosesAt));
    }

    [Fact]
    public void GivenOpensAtNotBeforeClosesAt_WhenCreatingPolicy_ShouldThrow()
    {
        Should.Throw<ArgumentException>(() => new BookingPolicy(
            [BookerType.PrivateIndividual],
            requiresReview: false,
            minDuration: TimeSpan.FromHours(1),
            maxDuration: TimeSpan.FromHours(4),
            opensAt: ClosesAt,
            closesAt: OpensAt));
    }

    [Fact]
    public void GivenNoAllowedBookerTypes_WhenCreatingPolicy_ShouldThrow()
    {
        Should.Throw<ArgumentException>(() => CreatePolicy(allowedBookerTypes: []));
    }

    [Fact]
    public void GivenNullAllowedBookerTypes_WhenCreatingPolicy_ShouldThrow()
    {
        Should.Throw<ArgumentNullException>(() => new BookingPolicy(
            null!,
            requiresReview: false,
            minDuration: TimeSpan.FromHours(1),
            maxDuration: TimeSpan.FromHours(4),
            opensAt: OpensAt,
            closesAt: ClosesAt));
    }

    [Fact]
    public void GivenNoBooker_WhenCheckingPolicy_ShouldThrow()
    {
        var policy = CreatePolicy();

        Should.Throw<ArgumentNullException>(() => policy.Allows(null!, Slot(10, 12), Now));
    }

    [Fact]
    public void GivenNoTimeslot_WhenCheckingPolicy_ShouldThrow()
    {
        var policy = CreatePolicy();

        Should.Throw<ArgumentNullException>(() => policy.Allows(PrivateBooker, null!, Now));
    }

    [Fact]
    public void GivenUndefinedBookerType_WhenCreatingPolicy_ShouldThrow()
    {
        Should.Throw<ArgumentException>(() =>
            CreatePolicy(allowedBookerTypes: [BookerType.PrivateIndividual, (BookerType)999]));
    }

    [Fact]
    public void GivenAllowedBookerAndValidTimeslot_WhenCheckingPolicy_ShouldAllow()
    {
        var policy = CreatePolicy();

        policy.Allows(PrivateBooker, Slot(10, 12), Now).ShouldBeTrue();
    }

    [Fact]
    public void GivenBookerTypeNotAllowed_WhenCheckingPolicy_ShouldDeny()
    {
        var policy = CreatePolicy();

        policy.Allows(AssociationBooker, Slot(10, 12), Now).ShouldBeFalse();
    }

    [Fact]
    public void GivenTimeslotShorterThanMinDuration_WhenCheckingPolicy_ShouldDeny()
    {
        var policy = CreatePolicy();
        var timeslot = new Timeslot(new DateTimeOffset(2030, 1, 2, 10, 0, 0, TimeSpan.Zero), new DateTimeOffset(2030, 1, 2, 10, 30, 0, TimeSpan.Zero));

        policy.Allows(PrivateBooker, timeslot, Now).ShouldBeFalse();
    }

    [Fact]
    public void GivenTimeslotLongerThanMaxDuration_WhenCheckingPolicy_ShouldDeny()
    {
        var policy = CreatePolicy();

        policy.Allows(PrivateBooker, Slot(10, 15), Now).ShouldBeFalse();
    }

    [Fact]
    public void GivenTimeslotExactlyMinAndMaxDuration_WhenCheckingPolicy_ShouldAllow()
    {
        var policy = CreatePolicy();

        policy.Allows(PrivateBooker, Slot(10, 11), Now).ShouldBeTrue();
        policy.Allows(PrivateBooker, Slot(10, 14), Now).ShouldBeTrue();
    }

    [Fact]
    public void GivenTimeslotStartingBeforeOpeningHours_WhenCheckingPolicy_ShouldDeny()
    {
        var policy = CreatePolicy();

        policy.Allows(PrivateBooker, Slot(7, 9), Now).ShouldBeFalse();
    }

    [Fact]
    public void GivenTimeslotEndingAfterClosingHours_WhenCheckingPolicy_ShouldDeny()
    {
        var policy = CreatePolicy();

        policy.Allows(PrivateBooker, Slot(21, 23), Now).ShouldBeFalse();
    }

    [Fact]
    public void GivenTimeslotWithOffset_WhenCheckingOpeningHours_ShouldUseLocalTimeOfTimeslot()
    {
        var policy = CreatePolicy();
        var offset = TimeSpan.FromHours(-2);
        var timeslot = new Timeslot(
            new DateTimeOffset(2030, 1, 2, 20, 0, 0, offset),
            new DateTimeOffset(2030, 1, 2, 22, 0, 0, offset));

        policy.Allows(PrivateBooker, timeslot, Now).ShouldBeTrue();
    }

    [Fact]
    public void GivenTimeslotInThePast_WhenCheckingPolicy_ShouldDeny()
    {
        var policy = CreatePolicy();
        var now = new DateTimeOffset(2030, 1, 3, 8, 0, 0, TimeSpan.Zero);

        policy.Allows(PrivateBooker, Slot(10, 12), now).ShouldBeFalse();
    }
}
