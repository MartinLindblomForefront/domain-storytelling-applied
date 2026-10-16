using TE.DomainStorytellingApplied.Domain;

namespace TE.DomainStorytellingApplied.Domain.Tests;

public class BookerTests
{
    [Fact]
    public void GivenValidDetails_WhenCreating_ShouldKeepDetails()
    {
        var bookerId = Id.Generate();
        var booker = new Booker(bookerId, BookerName.Create("Anna Andersson"));

        booker.BookerId.ShouldBe(bookerId);
        booker.Name.ShouldBe(BookerName.Create("Anna Andersson"));
    }
}