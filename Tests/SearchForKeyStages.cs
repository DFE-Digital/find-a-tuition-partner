using Microsoft.AspNetCore.Mvc;
using UI.Pages.FindATuitionPartner;

namespace Tests;

[Collection(nameof(SliceFixture))]
public class SearchForKeyStages : IAsyncLifetime
{
    private readonly SliceFixture fixture;

    public SearchForKeyStages(SliceFixture fixture) => this.fixture = fixture;

    public Task InitializeAsync() => fixture.ResetDatabase();
    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task Displays_all_key_stages()
    {
        var result = await fixture.SendAsync(new KeyStages.Query());

        result.AllKeyStages.Should().BeEquivalentTo(new[]
        {
            new { Name = KeyStage.KeyStage1 },
            new { Name = KeyStage.KeyStage2 },
            new { Name = KeyStage.KeyStage3 },
            new { Name = KeyStage.KeyStage4 },
        });
    }

    [Fact]
    public async Task Preserves_selected_from_querystring()
    {
        var result = await fixture.SendAsync(new KeyStages.Query
        {
            KeyStages = new[] { KeyStage.KeyStage1, KeyStage.KeyStage3 }
        });

        result.AllKeyStages.Should().BeEquivalentTo(new[]
        {
            new { Name = KeyStage.KeyStage1, Selected = true },
            new { Name = KeyStage.KeyStage2, Selected = false },
            new { Name = KeyStage.KeyStage3, Selected = true },
            new { Name = KeyStage.KeyStage4, Selected = false },
        });
    }

    [Fact]
    public async Task Preserves_other_search_parameters()
    {
        var query = new KeyStages.Query
        {
            Postcode = "123456",
        };

        var result = await fixture.SendAsync(query);

        result.Should().BeEquivalentTo(new
        {
            Postcode = "123456",
        });
    }

    [Fact]
    public async Task Updates_selection()
    {
        var result = await fixture.GetPage<KeyStages>().Execute(page =>
        {
            page.Data.Postcode = "AB00BA";
            page.Data.Subjects = new List<string>
            {
                "English", "Humanities",
            };
            return page.OnPost();
        });

        var resultPage = result.Should().BeAssignableTo<RedirectToPageResult>().Which;
        resultPage.PageName.Should().Be("Results");
        resultPage.RouteValues.Should().ContainKey("Postcode")
            .WhoseValue.Should().Be("AB00BA");
        resultPage.RouteValues.Should().ContainKey("Subjects")
            .WhoseValue.Should().BeEquivalentTo(new[]
            {
                "English", "Humanities",
            });
    }
}
