namespace TE.DomainStorytellingApplied.Domain.Tests;

public class ApproverTests
{
    [Fact]
    public void WhenCreatingApproverWithValidParameters_ShouldCreateApprover()
    {
        var id = new Id("a1");
        var approver = new Approver(id, "Alice");

        approver.Id.ShouldBe(id);
        approver.Name.ShouldBe("Alice");
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public void WhenCreatingApproverWithBlankName_ShouldThrowException(string name)
    {
        Should.Throw<ArgumentException>(() => new Approver(new Id("a1"), name));
    }
}
