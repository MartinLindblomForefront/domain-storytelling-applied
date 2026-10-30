using TE.DomainStorytellingApplied.Domain.Bookers;

namespace TE.DomainStorytellingApplied.Domain.Booking;

public static class SampleData
{
    public static BookingSystem WithSampleData(this BookingSystem system)
    {
        var gym = system.AddPremise("Gymnastiksalen");
        var hall = system.AddPremise("Aulan");
        var meetingRoom = system.AddPremise("Konferensrum A");

        var employee = system.AddBooker("Erik", "Lindqvist", "070-1112233", "erik.lindqvist@kommun.se", new MunicipalityEmployee());
        var privatePerson = system.AddBooker("Sara", "Nilsson", "073-4445566", "sara.nilsson@example.com", new PrivateIndividual());
        var representative = system.AddBooker("Johan", "Berg", "076-7778899", "johan.berg@ifk.se", new AssociationRepresentative("IFK Göteborg"));

        var gymMorning = system.AddTimeslot(gym.Id, At(13, 8), At(13, 10));
        system.AddTimeslot(gym.Id, At(13, 18), At(13, 20));
        var hallEvening = system.AddTimeslot(hall.Id, At(14, 17), At(14, 21));
        var meetingLunch = system.AddTimeslot(meetingRoom.Id, At(15, 11), At(15, 13));
        system.AddTimeslot(meetingRoom.Id, At(15, 14), At(15, 16));

        system.Reserve(representative.Id, gymMorning.Id);
        system.Reserve(privatePerson.Id, hallEvening.Id);
        system.Reserve(employee.Id, meetingLunch.Id);

        return system;
    }

    private static DateTime At(int day, int hour) => new(2026, 10, day, hour, 0, 0);
}
