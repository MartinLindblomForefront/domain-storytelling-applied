namespace TE.DomainStorytellingApplied.Domain;

/// <summary>
/// A domain service: answers a question that no single booking can answer.
/// "Is this venue free at this time?" needs all the bookings for the venue, so the rule lives here
/// and not in Booking. It has no data of its own, which is why it is static.
/// </summary>
public static class VenueAvailabilityService
{
    public static bool IsAvailable(Guid venueId, TimeSlot timeSlot, IEnumerable<Booking> existingBookings)
    {
        foreach (var booking in existingBookings)
        {
            var isForThisVenue = booking.VenueId == venueId;

            // A cancelled or expired booking has given its time back.
            if (isForThisVenue && booking.IsHoldingItsTime && booking.TimeSlot.OverlapsWith(timeSlot))
            {
                return false;
            }
        }

        return true;
    }
}
