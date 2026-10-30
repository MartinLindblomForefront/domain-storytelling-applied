namespace TE.DomainStorytellingApplied.Domain.Tests;

public class BookingTests
{
    // The moment the booking is reserved, eight days before the slot.
    private static readonly DateTime Now = TestData.Now;

    // One hour, in the middle of the day.
    private static readonly TimeSlot Slot = new(
        new DateTime(2026, 10, 20, 10, 0, 0, DateTimeKind.Utc),
        new DateTime(2026, 10, 20, 11, 0, 0, DateTimeKind.Utc));

    private static readonly ContactDetails Contact = TestData.Contact;

    // 06:00 to 07:00, before the venue opens at 08:00.
    private static readonly TimeSlot BeforeOpening = new(
        new DateTime(2026, 10, 20, 6, 0, 0, DateTimeKind.Utc),
        new DateTime(2026, 10, 20, 7, 0, 0, DateTimeKind.Utc));

    // Starts at 14:00 the same day as Slot, and lasts the given number of hours.
    private static TimeSlot AfternoonSlot(int hours)
    {
        var start = new DateTime(2026, 10, 20, 14, 0, 0, DateTimeKind.Utc);

        return new TimeSlot(start, start.AddHours(hours));
    }

    // A valid booking with a fee of 400 SEK, reserved at Now.
    private static Booking ReserveBooking()
    {
        return TestData.ReserveBooking(TestData.CreateVenue(), Slot);
    }

    // The same booking, paid straight away.
    private static Booking ReserveAndPayBooking()
    {
        var booking = ReserveBooking();
        booking.Pay(SwishPayment(400m, Now), Now);

        return booking;
    }

    private static Payment SwishPayment(decimal amount, DateTime paidAt)
    {
        return new Payment(new Money(amount, "SEK"), PaymentMethod.Swish, paidAt);
    }

    // ----- Reserve -----

    [Fact]
    public void GivenValidInput_WhenReserving_ShouldCreateABookingForThatVenue()
    {
        var venue = TestData.CreateVenue();

        var booking = Booking.Reserve(venue, BookerType.Private, Slot, 10, Contact, Now);

        booking.Id.ShouldNotBe(Guid.Empty);
        booking.VenueId.ShouldBe(venue.Id);
        booking.BookerType.ShouldBe(BookerType.Private);
        booking.TimeSlot.ShouldBe(Slot);
        booking.ParticipantCount.ShouldBe(10);
        booking.Contact.ShouldBe(Contact);
    }

    [Fact]
    public void GivenAPrivateBookerAndAOneHourSlot_WhenReserving_ShouldGetTheVenuesFeeForThatSlot()
    {
        var booking = Booking.Reserve(TestData.CreateVenue(), BookerType.Private, Slot, 10, Contact, Now);

        booking.Fee.ShouldBe(new Money(400m, "SEK"));
    }

    [Fact]
    public void GivenABookerTypeTheVenueDoesNotAllow_WhenReserving_ShouldThrow()
    {
        Should.Throw<ArgumentException>(() => Booking.Reserve(TestData.CreateVenue(), BookerType.Region, Slot, 10, Contact, Now));
    }

    [Fact]
    public void GivenADeactivatedVenue_WhenReserving_ShouldThrow()
    {
        var venue = TestData.CreateVenue();
        venue.Deactivate();

        Should.Throw<ArgumentException>(() => Booking.Reserve(venue, BookerType.Private, Slot, 10, Contact, Now));
    }

    [Fact]
    public void GivenATimeSlotOutsideOpeningHours_WhenReserving_ShouldThrow()
    {
        Should.Throw<ArgumentException>(() => Booking.Reserve(TestData.CreateVenue(), BookerType.Private, BeforeOpening, 10, Contact, Now));
    }

    [Fact]
    public void GivenNoParticipants_WhenReserving_ShouldThrow()
    {
        Should.Throw<ArgumentException>(() => Booking.Reserve(TestData.CreateVenue(), BookerType.Private, Slot, 0, Contact, Now));
    }

    [Fact]
    public void GivenOneParticipant_WhenReserving_ShouldBeAllowed()
    {
        var booking = Booking.Reserve(TestData.CreateVenue(), BookerType.Private, Slot, 1, Contact, Now);

        booking.ParticipantCount.ShouldBe(1);
    }

    [Fact]
    public void GivenMoreParticipantsThanCapacity_WhenReserving_ShouldThrow()
    {
        Should.Throw<ArgumentException>(() => Booking.Reserve(TestData.CreateVenue(), BookerType.Private, Slot, 31, Contact, Now));
    }

    [Fact]
    public void GivenExactlyAsManyParticipantsAsCapacity_WhenReserving_ShouldBeAllowed()
    {
        var booking = Booking.Reserve(TestData.CreateVenue(), BookerType.Private, Slot, 30, Contact, Now);

        booking.ParticipantCount.ShouldBe(30);
    }

    [Fact]
    public void GivenANewBooking_WhenReserving_ShouldBeReservedAndHeldForFifteenMinutes()
    {
        var booking = ReserveBooking();

        booking.Status.ShouldBe(BookingStatus.Reserved);
        booking.ReservedUntil.ShouldBe(Now.AddMinutes(15));
        booking.Payment.ShouldBeNull();
    }

    // ----- Pay -----

    [Fact]
    public void GivenAPaymentWithinTheHoldTime_WhenPaying_ShouldConfirmTheBooking()
    {
        var booking = ReserveBooking();
        var paidAt = Now.AddMinutes(5);
        var payment = SwishPayment(400m, paidAt);

        booking.Pay(payment, paidAt);

        booking.Status.ShouldBe(BookingStatus.Confirmed);
        booking.Payment.ShouldBe(payment);
    }

    [Fact]
    public void GivenAPaymentExactlyWhenTheHoldEnds_WhenPaying_ShouldBeAllowed()
    {
        var booking = ReserveBooking();
        var paidAt = Now.AddMinutes(15);

        booking.Pay(SwishPayment(400m, paidAt), paidAt);

        booking.Status.ShouldBe(BookingStatus.Confirmed);
    }

    [Fact]
    public void GivenAPaymentAfterTheHoldTime_WhenPaying_ShouldThrow()
    {
        var booking = ReserveBooking();
        var paidAt = Now.AddMinutes(16);

        Should.Throw<InvalidOperationException>(() => booking.Pay(SwishPayment(400m, paidAt), paidAt));
    }

    [Fact]
    public void GivenAPaymentWithTheWrongAmount_WhenPaying_ShouldThrow()
    {
        var booking = ReserveBooking();

        Should.Throw<ArgumentException>(() => booking.Pay(SwishPayment(399m, Now), Now));
    }

    [Fact]
    public void GivenAnAlreadyPaidBooking_WhenPayingAgain_ShouldThrow()
    {
        var booking = ReserveAndPayBooking();

        Should.Throw<InvalidOperationException>(() => booking.Pay(SwishPayment(400m, Now), Now));
    }

    [Fact]
    public void GivenAPaymentWithTheWrongAmount_WhenPaying_ShouldLeaveTheBookingReserved()
    {
        var booking = ReserveBooking();

        Should.Throw<ArgumentException>(() => booking.Pay(SwishPayment(399m, Now), Now));

        booking.Status.ShouldBe(BookingStatus.Reserved);
        booking.Payment.ShouldBeNull();
    }

    // ----- Domain events -----

    [Fact]
    public void GivenANewBooking_WhenReserving_ShouldHaveNoDomainEvents()
    {
        var booking = ReserveBooking();

        booking.DomainEvents.ShouldBeEmpty();
    }

    [Fact]
    public void GivenAPaymentWithinTheHoldTime_WhenPaying_ShouldRecordBookingConfirmed()
    {
        var booking = ReserveBooking();

        booking.Pay(SwishPayment(400m, Now), Now);

        var confirmed = booking.DomainEvents.ShouldHaveSingleItem().ShouldBeOfType<BookingConfirmed>();
        confirmed.BookingId.ShouldBe(booking.Id);
        confirmed.ContactEmail.ShouldBe(Contact.Email);
        confirmed.Fee.ShouldBe(new Money(400m, "SEK"));
    }

    [Fact]
    public void GivenAPaymentWithTheWrongAmount_WhenPaying_ShouldNotRecordAnEvent()
    {
        var booking = ReserveBooking();

        Should.Throw<ArgumentException>(() => booking.Pay(SwishPayment(399m, Now), Now));

        booking.DomainEvents.ShouldBeEmpty();
    }

    // ----- Cancel -----

    [Fact]
    public void GivenAReservedBooking_WhenCancelling_ShouldBeCancelledAndNotLate()
    {
        var booking = ReserveBooking();

        booking.Cancel(Now);

        booking.Status.ShouldBe(BookingStatus.Cancelled);
        booking.IsLateCancellation.ShouldBeFalse();
    }

    [Fact]
    public void GivenAConfirmedBookingMoreThan48HoursBeforeStart_WhenCancelling_ShouldNotBeLate()
    {
        var booking = ReserveAndPayBooking();

        booking.Cancel(Slot.Start.AddHours(-49));

        booking.Status.ShouldBe(BookingStatus.Cancelled);
        booking.IsLateCancellation.ShouldBeFalse();
    }

    [Fact]
    public void GivenAConfirmedBookingExactly48HoursBeforeStart_WhenCancelling_ShouldNotBeLate()
    {
        var booking = ReserveAndPayBooking();

        booking.Cancel(Slot.Start.AddHours(-48));

        booking.IsLateCancellation.ShouldBeFalse();
    }

    [Fact]
    public void GivenAConfirmedBookingLessThan48HoursBeforeStart_WhenCancelling_ShouldBeLate()
    {
        var booking = ReserveAndPayBooking();

        booking.Cancel(Slot.Start.AddHours(-47));

        booking.Status.ShouldBe(BookingStatus.Cancelled);
        booking.IsLateCancellation.ShouldBeTrue();
    }

    [Fact]
    public void GivenAnUnpaidBookingLessThan48HoursBeforeStart_WhenCancelling_ShouldNotBeLate()
    {
        var booking = ReserveBooking();

        // Still Reserved this long after the hold ran out, because nothing has called Expire.
        booking.Cancel(Slot.Start.AddHours(-1));

        booking.IsLateCancellation.ShouldBeFalse();
    }

    [Fact]
    public void GivenACancelledBooking_WhenCancellingAgain_ShouldThrow()
    {
        var booking = ReserveBooking();
        booking.Cancel(Now);

        Should.Throw<InvalidOperationException>(() => booking.Cancel(Now));
    }

    [Fact]
    public void GivenACancelledBooking_WhenPaying_ShouldThrow()
    {
        var booking = ReserveBooking();
        booking.Cancel(Now);

        Should.Throw<InvalidOperationException>(() => booking.Pay(SwishPayment(400m, Now), Now));
    }

    // ----- Change time slot -----

    [Fact]
    public void GivenANewSlotOfTheSameLength_WhenChangingTimeSlot_ShouldReplaceItAndStayTheSameBooking()
    {
        var venue = TestData.CreateVenue();
        var booking = TestData.ReserveBooking(venue, Slot);
        var idBefore = booking.Id;
        var laterSlot = AfternoonSlot(hours: 1);

        booking.ChangeTimeSlot(venue, laterSlot);

        booking.TimeSlot.ShouldBe(laterSlot);
        booking.Id.ShouldBe(idBefore);
        booking.Fee.ShouldBe(new Money(400m, "SEK"));
    }

    [Fact]
    public void GivenAnUnpaidBookingAndALongerSlot_WhenChangingTimeSlot_ShouldWorkOutTheFeeAgain()
    {
        var venue = TestData.CreateVenue();
        var booking = TestData.ReserveBooking(venue, Slot);
        var twoHours = AfternoonSlot(hours: 2);

        booking.ChangeTimeSlot(venue, twoHours);

        booking.TimeSlot.ShouldBe(twoHours);
        booking.Fee.ShouldBe(new Money(800m, "SEK"));
    }

    [Fact]
    public void GivenAPaidBookingAndASlotThatCostsTheSame_WhenChangingTimeSlot_ShouldBeAllowed()
    {
        var venue = TestData.CreateVenue();
        var booking = TestData.ReserveBooking(venue, Slot);
        booking.Pay(SwishPayment(400m, Now), Now);
        var laterSlot = AfternoonSlot(hours: 1);

        booking.ChangeTimeSlot(venue, laterSlot);

        booking.TimeSlot.ShouldBe(laterSlot);
        booking.Status.ShouldBe(BookingStatus.Confirmed);
    }

    [Fact]
    public void GivenAPaidBookingAndASlotThatCostsMore_WhenChangingTimeSlot_ShouldThrowAndKeepTheOldSlotAndFee()
    {
        var venue = TestData.CreateVenue();
        var booking = TestData.ReserveBooking(venue, Slot);
        booking.Pay(SwishPayment(400m, Now), Now);

        Should.Throw<ArgumentException>(() => booking.ChangeTimeSlot(venue, AfternoonSlot(hours: 2)));

        booking.TimeSlot.ShouldBe(Slot);
        booking.Fee.ShouldBe(new Money(400m, "SEK"));
    }

    [Fact]
    public void GivenANewSlotOutsideOpeningHours_WhenChangingTimeSlot_ShouldThrowAndKeepTheOldSlot()
    {
        var venue = TestData.CreateVenue();
        var booking = TestData.ReserveBooking(venue, Slot);
        Should.Throw<ArgumentException>(() => booking.ChangeTimeSlot(venue, BeforeOpening));

        booking.TimeSlot.ShouldBe(Slot);
    }

    [Fact]
    public void GivenAnotherVenue_WhenChangingTimeSlot_ShouldThrow()
    {
        var booking = ReserveBooking();
        var otherVenue = TestData.CreateVenue();

        Should.Throw<ArgumentException>(() => booking.ChangeTimeSlot(otherVenue, AfternoonSlot(hours: 1)));
    }

    [Fact]
    public void GivenACancelledBooking_WhenChangingTimeSlot_ShouldThrow()
    {
        var venue = TestData.CreateVenue();
        var booking = TestData.ReserveBooking(venue, Slot);
        booking.Cancel(Now);

        Should.Throw<InvalidOperationException>(() => booking.ChangeTimeSlot(venue, AfternoonSlot(hours: 1)));
    }

    [Fact]
    public void GivenAnExpiredBooking_WhenChangingTimeSlot_ShouldThrow()
    {
        var venue = TestData.CreateVenue();
        var booking = TestData.ReserveBooking(venue, Slot);
        booking.Expire(Now.AddMinutes(16));

        Should.Throw<InvalidOperationException>(() => booking.ChangeTimeSlot(venue, AfternoonSlot(hours: 1)));
    }

    // ----- Expire -----

    [Fact]
    public void GivenAnUnpaidBookingAfterTheHoldTime_WhenExpiring_ShouldBeExpired()
    {
        var booking = ReserveBooking();

        booking.Expire(Now.AddMinutes(16));

        booking.Status.ShouldBe(BookingStatus.Expired);
    }

    [Fact]
    public void GivenAnUnpaidBookingExactlyWhenTheHoldEnds_WhenExpiring_ShouldStayReserved()
    {
        var booking = ReserveBooking();

        booking.Expire(Now.AddMinutes(15));

        booking.Status.ShouldBe(BookingStatus.Reserved);
    }

    [Fact]
    public void GivenAnUnpaidBookingWithinTheHoldTime_WhenExpiring_ShouldStayReserved()
    {
        var booking = ReserveBooking();

        booking.Expire(Now.AddMinutes(5));

        booking.Status.ShouldBe(BookingStatus.Reserved);
    }

    [Fact]
    public void GivenAConfirmedBooking_WhenExpiring_ShouldStayConfirmed()
    {
        var booking = ReserveAndPayBooking();

        booking.Expire(Now.AddMinutes(16));

        booking.Status.ShouldBe(BookingStatus.Confirmed);
    }

    [Fact]
    public void GivenACancelledBooking_WhenExpiring_ShouldStayCancelled()
    {
        var booking = ReserveBooking();
        booking.Cancel(Now);

        booking.Expire(Now.AddMinutes(16));

        booking.Status.ShouldBe(BookingStatus.Cancelled);
    }

    [Fact]
    public void GivenAnExpiredBooking_WhenPaying_ShouldThrow()
    {
        var booking = ReserveBooking();
        booking.Expire(Now.AddMinutes(16));

        // Paid at Now, inside the hold time, so only the status stops the payment.
        Should.Throw<InvalidOperationException>(() => booking.Pay(SwishPayment(400m, Now), Now));
    }

    [Fact]
    public void GivenAnExpiredBooking_WhenCancelling_ShouldThrow()
    {
        var booking = ReserveBooking();
        var later = Now.AddMinutes(16);
        booking.Expire(later);

        Should.Throw<InvalidOperationException>(() => booking.Cancel(later));
    }
}
