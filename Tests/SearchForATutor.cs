using Infrastructure.Entities;
using Microsoft.AspNetCore.Mvc;
using UI.Pages.FindTuition;

namespace Tests;

[Collection(nameof(SliceFixture))]
public class SearchForATutor
{
    private readonly SliceFixture fixture;

    public SearchForATutor(SliceFixture fixture) => this.fixture = fixture;

    [Fact]
    public async void Starting_a_search_creates_a_search_record()
    {
        var search = await fixture.SendAsync(new Search.Command { });

        var savedSearch = await fixture.GetDbAsync<UserSearch>();
        //savedSearch.Should().ContainEntity(search.SearchId);
        savedSearch.Should().ContainEquivalentOf(new
        {
            Id = search.SearchId
        });
    }

    [Fact]
    public async void Starting_a_search_redirects_to_location()
    {
        var result = await fixture.GetPage<Search, IActionResult>(x => x.OnGet());

        var redirect = result.Should().BeOfType<RedirectToPageResult>().Which;
        redirect.PageName.Should().Be("Location");
        redirect.RouteValues.Should().ContainKey("SearchId");
        (await fixture.GetDbAsync<UserSearch>()).Should().ContainEquivalentOf(new
        {
            Id = redirect.RouteValues!["SearchId"]
        });
    }
}