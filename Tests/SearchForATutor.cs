using Infrastructure.Entities;
using UI.Pages.FindTuition;

namespace Tests;

[Collection(nameof(SliceFixture))]
public class SearchForATutor
{
    [Fact]
    public async void Starting_a_search_creates_a_search_record()
    {
        var fixture = new SliceFixture();

        var search = await fixture.SendAsync(new Search.Command { });

        var savedSearch = await fixture.GetDbAsync<UserSearch>();
        savedSearch.Should().ContainEquivalentOf(new
        {
            Id = search.SearchId
        });
    }
}
