using TE.DomainStorytellingApplied.Domain;

namespace TE.DomainStorytellingApplied.Domain.Tests;

public class PremisesTests
{
    [Fact]
    public void GivenValidDetails_WhenCreating_ShouldKeepDetails()
    {
        var premisesId = Id.Generate();
        string[] equipment = ["Projector", "Whiteboard"];

        var premises = new Premises(
            premisesId,
            PremisesName.Create("Conference Room"),
            Address.Create("Main Street 1"),
            Capacity.Create(12),
            equipment);

        premises.PremisesId.ShouldBe(premisesId);
        premises.Name.ShouldBe(PremisesName.Create("Conference Room"));
        premises.Address.ShouldBe(Address.Create("Main Street 1"));
        premises.Capacity.ShouldBe(Capacity.Create(12));
        premises.Equipment.ShouldBe(equipment);
    }
}