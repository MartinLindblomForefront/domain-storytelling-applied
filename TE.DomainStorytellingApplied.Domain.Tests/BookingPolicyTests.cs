using TE.DomainStorytellingApplied.Domain.Reservations;

namespace TE.DomainStorytellingApplied.Domain.Tests;

public class BookingPolicyTests
{
    [Fact]
    public void DetermineApprovalMode_WhenDurationIsUnderEightHours_ReturnsAutomatic()
    {
        var approvalMode = BookingPolicy.DetermineApprovalMode(
            new DateOnly(2026, 10, 8),
            TimeSlot.Create(
                new DateTimeOffset(2026, 10, 10, 9, 0, 0, TimeSpan.Zero),
                new DateTimeOffset(2026, 10, 10, 16, 59, 0, TimeSpan.Zero)));

        approvalMode.ShouldBe(ApprovalMode.Automatic);
    }

    [Fact]
    public void DetermineApprovalMode_WhenDurationIsEightHoursOrMore_ReturnsManualReview()
    {
        var approvalMode = BookingPolicy.DetermineApprovalMode(
            new DateOnly(2026, 10, 8),
            TimeSlot.Create(
                new DateTimeOffset(2026, 10, 10, 9, 0, 0, TimeSpan.Zero),
                new DateTimeOffset(2026, 10, 10, 17, 0, 0, TimeSpan.Zero)));

        approvalMode.ShouldBe(ApprovalMode.ManualReview);
    }

    [Fact]
    public void DetermineApprovalMode_WhenDurationIsUnderOneHour_Throws()
    {
        Should.Throw<ArgumentOutOfRangeException>(() =>
            BookingPolicy.DetermineApprovalMode(
                new DateOnly(2026, 10, 8),
                TimeSlot.Create(
                    new DateTimeOffset(2026, 10, 10, 9, 0, 0, TimeSpan.Zero),
                    new DateTimeOffset(2026, 10, 10, 9, 59, 0, TimeSpan.Zero))));
    }

    [Fact]
    public void DetermineApprovalMode_WhenBookingIsLessThanTwoCalendarDaysAhead_Throws()
    {
        Should.Throw<ArgumentOutOfRangeException>(() =>
            BookingPolicy.DetermineApprovalMode(
                new DateOnly(2026, 10, 8),
                TimeSlot.Create(
                    new DateTimeOffset(2026, 10, 9, 23, 59, 0, TimeSpan.FromHours(2)),
                    new DateTimeOffset(2026, 10, 10, 1, 0, 0, TimeSpan.FromHours(2)))));
    }

    [Fact]
    public void DetermineApprovalMode_WhenBookingIsExactlyTwoCalendarDaysAhead_IsAllowed()
    {
        var approvalMode = BookingPolicy.DetermineApprovalMode(
            new DateOnly(2026, 10, 8),
            TimeSlot.Create(
                new DateTimeOffset(2026, 10, 10, 0, 0, 0, TimeSpan.FromHours(2)),
                new DateTimeOffset(2026, 10, 10, 1, 0, 0, TimeSpan.FromHours(2))));

        approvalMode.ShouldBe(ApprovalMode.Automatic);
    }

    [Fact]
    public void DetermineApprovalMode_WhenBookingIsMoreThanOneYearAhead_Throws()
    {
        Should.Throw<ArgumentOutOfRangeException>(() =>
            BookingPolicy.DetermineApprovalMode(
                new DateOnly(2026, 10, 8),
                TimeSlot.Create(
                    new DateTimeOffset(2027, 10, 9, 9, 0, 0, TimeSpan.Zero),
                    new DateTimeOffset(2027, 10, 9, 10, 0, 0, TimeSpan.Zero))));
    }

    [Fact]
    public void DetermineApprovalMode_WhenBookingIsExactlyOneYearAhead_IsAllowed()
    {
        var approvalMode = BookingPolicy.DetermineApprovalMode(
            new DateOnly(2026, 10, 8),
            TimeSlot.Create(
                new DateTimeOffset(2027, 10, 8, 9, 0, 0, TimeSpan.FromHours(2)),
                new DateTimeOffset(2027, 10, 8, 10, 0, 0, TimeSpan.FromHours(2))));

        approvalMode.ShouldBe(ApprovalMode.Automatic);
    }

    [Fact]
    public void DetermineApprovalMode_UsesSwedishDateForBookingWindow()
    {
        var approvalMode = BookingPolicy.DetermineApprovalMode(
            new DateOnly(2026, 10, 8),
            TimeSlot.Create(
                new DateTimeOffset(2026, 10, 9, 23, 0, 0, TimeSpan.FromHours(-2)),
                new DateTimeOffset(2026, 10, 10, 0, 0, 0, TimeSpan.FromHours(-2))));

        approvalMode.ShouldBe(ApprovalMode.Automatic);
    }
}
