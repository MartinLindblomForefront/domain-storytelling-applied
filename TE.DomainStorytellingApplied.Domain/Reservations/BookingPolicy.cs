using TE.DomainStorytellingApplied.Domain.Premises;

namespace TE.DomainStorytellingApplied.Domain.Reservations;

public static class BookingPolicy
{
    private static readonly TimeSpan MinimumDuration = TimeSpan.FromHours(1);
    private static readonly TimeSpan ManualReviewThreshold = TimeSpan.FromHours(8);

    public static ApprovalMode DetermineApprovalMode(
        DateOnly requestedOn,
        TimeSlot timeSlot)
    {
        ArgumentNullException.ThrowIfNull(timeSlot);

        if (timeSlot.Duration < MinimumDuration)
        {
            throw new ArgumentOutOfRangeException(
                nameof(timeSlot),
                "Bokningen måste vara minst en timme.");
        }

        ValidateBookingWindow(
            requestedOn,
            SwedishBusinessCalendar.GetSwedishDate(timeSlot.StartsAt));

        return timeSlot.Duration < ManualReviewThreshold
            ? ApprovalMode.Automatic
            : ApprovalMode.ManualReview;
    }

    public static void EnsureAvailable(
        PremisesId premisesId,
        TimeSlot requestedTimeSlot,
        IEnumerable<Reservation> existingReservations)
    {
        ArgumentNullException.ThrowIfNull(premisesId);
        ArgumentNullException.ThrowIfNull(requestedTimeSlot);
        ArgumentNullException.ThrowIfNull(existingReservations);

        var conflict = existingReservations.Any(reservation =>
            reservation.PremisesId == premisesId &&
            reservation.Status is ReservationStatus.PendingApproval or ReservationStatus.Confirmed &&
            requestedTimeSlot.Overlaps(reservation.TimeSlot));

        if (conflict)
        {
            throw new InvalidOperationException(
                "Tidsluckan är redan bokad eller väntar på godkännande.");
        }
    }

    public static void ValidateBookingWindow(DateOnly requestedOn, DateOnly bookingDate)
    {
        var earliestAllowedDate = requestedOn.AddDays(2);
        var latestAllowedDate = requestedOn.AddYears(1);

        if (bookingDate < earliestAllowedDate)
        {
            throw new ArgumentOutOfRangeException(
                nameof(bookingDate),
                "Bokningen måste göras minst två kalenderdagar i förväg.");
        }

        if (bookingDate > latestAllowedDate)
        {
            throw new ArgumentOutOfRangeException(
                nameof(bookingDate),
                "Bokningen får göras högst ett år i förväg.");
        }
    }
}
