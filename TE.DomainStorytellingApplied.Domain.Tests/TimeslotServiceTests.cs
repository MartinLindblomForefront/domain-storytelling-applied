using TE.DomainStorytellingApplied.Domain.Tests.Fakes;

namespace TE.DomainStorytellingApplied.Domain.Tests;

public class TimeslotServiceTests
{
    private readonly InMemoryTimeslotRepository _timeslots = new();

    private TimeslotService NewService() => new(_timeslots);

    private static DateTimeOffset At(int hoursFromNow) => TestData.Now.AddHours(hoursFromNow);

    [Fact]
    public void WhenCreatingTimeslot_ShouldStoreAvailableTimeslotForPremises()
    {
        var premises = TestData.Premises();

        var timeslot = NewService().Create(premises, new Id("t1"), At(1), At(2));

        timeslot.PremisesId.ShouldBe(premises.Id);
        timeslot.IsReserved.ShouldBeFalse();
        _timeslots.Find(timeslot.Id).ShouldBe(timeslot);
    }

    [Fact]
    public void WhenCreatingOverlappingTimeslot_ShouldThrowException()
    {
        var premises = TestData.Premises();
        var service = NewService();
        service.Create(premises, new Id("t1"), At(1), At(3));

        Should.Throw<InvalidOperationException>(() => service.Create(premises, new Id("t2"), At(2), At(4)));

        _timeslots.FindByPremises(premises.Id).Count.ShouldBe(1);
    }

    [Fact]
    public void WhenCreatingAdjacentTimeslot_ShouldSucceed()
    {
        var premises = TestData.Premises();
        var service = NewService();
        service.Create(premises, new Id("t1"), At(1), At(2));

        service.Create(premises, new Id("t2"), At(2), At(3));

        _timeslots.FindByPremises(premises.Id).Count.ShouldBe(2);
    }

    [Fact]
    public void WhenCreatingSameTimeForOtherPremises_ShouldSucceed()
    {
        var service = NewService();
        service.Create(TestData.Premises("p1"), new Id("t1"), At(1), At(2));

        service.Create(TestData.Premises("p2"), new Id("t2"), At(1), At(2));

        _timeslots.FindByPremises(new Id("p2")).Count.ShouldBe(1);
    }

    [Fact]
    public void WhenCreatingTimeslotWithDuplicateId_ShouldThrowException()
    {
        var premises = TestData.Premises();
        var service = NewService();
        service.Create(premises, new Id("t1"), At(1), At(2));

        Should.Throw<InvalidOperationException>(() => service.Create(premises, new Id("t1"), At(5), At(6)));
    }
}
