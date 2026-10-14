namespace TE.DomainStorytellingApplied.Domain.Tests;

public class ApproverTests
{
    [Fact]
    public void GivenValidName_WhenCreatingApprover_ShouldSetProperties()
    {
        var approver = new Approver("Bertil Berg");

        approver.Id.ShouldNotBe(Guid.Empty);
        approver.Name.ShouldBe("Bertil Berg");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void GivenEmptyName_WhenCreatingApprover_ShouldThrow(string name)
    {
        Should.Throw<ArgumentException>(() => new Approver(name));
    }
}
