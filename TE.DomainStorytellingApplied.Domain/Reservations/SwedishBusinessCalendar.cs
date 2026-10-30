namespace TE.DomainStorytellingApplied.Domain.Reservations;

public static class SwedishBusinessCalendar
{
    private static readonly TimeZoneInfo SwedishTimeZone = TimeZoneInfo.FindSystemTimeZoneById(
        OperatingSystem.IsWindows() ? "W. Europe Standard Time" : "Europe/Stockholm");

    public static DateOnly GetSwedishDate(DateTimeOffset dateTime) =>
        DateOnly.FromDateTime(TimeZoneInfo.ConvertTime(dateTime, SwedishTimeZone).DateTime);

    public static DateTimeOffset GetCancellationDeadline(DateTimeOffset reservationStartsAt)
    {
        var localStartDate = GetSwedishDate(reservationStartsAt);

        var businessDaysCounted = 0;
        var deadlineDate = localStartDate;

        while (businessDaysCounted < 2)
        {
            deadlineDate = deadlineDate.AddDays(-1);
            if (IsBusinessDay(deadlineDate))
            {
                businessDaysCounted++;
            }
        }

        var localDeadline = deadlineDate.ToDateTime(new TimeOnly(12, 0));
        return new DateTimeOffset(
            TimeZoneInfo.ConvertTimeToUtc(localDeadline, SwedishTimeZone));
    }

    public static bool IsCancellationAllowed(
        DateTimeOffset reservationStartsAt,
        DateTimeOffset cancellationRequestedAt) =>
        cancellationRequestedAt < GetCancellationDeadline(reservationStartsAt);

    private static bool IsBusinessDay(DateOnly date)
    {
        if (date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
        {
            return false;
        }

        return !GetPublicHolidays(date.Year).Contains(date);
    }

    private static HashSet<DateOnly> GetPublicHolidays(int year)
    {
        var easterSunday = GetEasterSunday(year);
        return
        [
            new DateOnly(year, 1, 1),
            new DateOnly(year, 1, 6),
            new DateOnly(year, 5, 1),
            new DateOnly(year, 6, 6),
            new DateOnly(year, 12, 25),
            new DateOnly(year, 12, 26),
            easterSunday.AddDays(-2),
            easterSunday.AddDays(1),
            easterSunday.AddDays(39),
            GetMidsummerDay(year),
            GetAllSaintsDay(year)
        ];
    }

    private static DateOnly GetEasterSunday(int year)
    {
        var a = year % 19;
        var b = year / 100;
        var c = year % 100;
        var d = b / 4;
        var e = b % 4;
        var f = (b + 8) / 25;
        var g = (b - f + 1) / 3;
        var h = (19 * a + b - d - g + 15) % 30;
        var i = c / 4;
        var k = c % 4;
        var l = (32 + 2 * e + 2 * i - h - k) % 7;
        var m = (a + 11 * h + 22 * l) / 451;
        var month = (h + l - 7 * m + 114) / 31;
        var day = (h + l - 7 * m + 114) % 31 + 1;

        return new DateOnly(year, month, day);
    }

    private static DateOnly GetMidsummerDay(int year)
    {
        for (var day = 20; day <= 26; day++)
        {
            var date = new DateOnly(year, 6, day);
            if (date.DayOfWeek == DayOfWeek.Saturday)
            {
                return date;
            }
        }

        throw new InvalidOperationException("Could not determine Swedish Midsummer Day.");
    }

    private static DateOnly GetAllSaintsDay(int year)
    {
        for (var day = 31; day <= 31 + 6; day++)
        {
            var date = new DateOnly(year, 10, 1).AddDays(day - 1);
            if (date.DayOfWeek == DayOfWeek.Saturday)
            {
                return date;
            }
        }

        throw new InvalidOperationException("Could not determine Swedish All Saints' Day.");
    }
}
