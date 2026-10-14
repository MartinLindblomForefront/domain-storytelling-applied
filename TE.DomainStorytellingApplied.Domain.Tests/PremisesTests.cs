namespace TE.DomainStorytellingApplied.Domain.Tests;

public class PremisesTests
{
    private static readonly DateTimeOffset Morning = new(2026, 10, 9, 8, 0, 0, TimeSpan.Zero);

    private static readonly Timeslot FirstSlot = new(Morning, Morning.AddHours(1));
    private static readonly Timeslot SecondSlot = new(Morning.AddHours(1), Morning.AddHours(2));

    private static Premises CreatePremises() =>
        new(Id.Generate(), "Slöjdsal", 200m, [FirstSlot, SecondSlot]);

    [Fact]
    public void GivenEmptyName_WhenCreating_ShouldThrow()
    {
        Should.Throw<Exception>(() => new Premises(Id.Generate(), "", 200m, [FirstSlot]));
    }

    [Fact]
    public void GivenNegativePrice_WhenCreating_ShouldThrow()
    {
        Should.Throw<Exception>(() => new Premises(Id.Generate(), "Slöjdsal", -1m, [FirstSlot]));
    }

    [Fact]
    public void GivenOfferedTimeslot_WhenReserving_ShouldBeReserved()
    {
        var premises = CreatePremises();

        premises.Reserve(FirstSlot);

        premises.IsReserved(FirstSlot).ShouldBeTrue();
        premises.IsReserved(SecondSlot).ShouldBeFalse();
    }

    [Fact]
    public void GivenTimeslotNotOffered_WhenReserving_ShouldThrow()
    {
        var premises = CreatePremises();
        var otherSlot = new Timeslot(Morning.AddHours(5), Morning.AddHours(6));

        Should.Throw<Exception>(() => premises.Reserve(otherSlot));
    }

    [Fact]
    public void GivenAlreadyReservedTimeslot_WhenReserving_ShouldThrow()
    {
        var premises = CreatePremises();
        premises.Reserve(FirstSlot);

        Should.Throw<Exception>(() => premises.Reserve(FirstSlot));
    }

    [Fact]
    public void GivenReservedTimeslot_WhenReleasing_ShouldBeReservableAgain()
    {
        var premises = CreatePremises();
        premises.Reserve(FirstSlot);

        premises.Release(FirstSlot);

        premises.IsReserved(FirstSlot).ShouldBeFalse();
        Should.NotThrow(() => premises.Reserve(FirstSlot));
    }

    [Fact]
    public void GivenTimeslotNotReserved_WhenReleasing_ShouldThrow()
    {
        var premises = CreatePremises();

        Should.Throw<Exception>(() => premises.Release(FirstSlot));
    }
}
