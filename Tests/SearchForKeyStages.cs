using Microsoft.AspNetCore.Mvc;
using UI.Pages;

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
        var result = await fixture.SendAsync(new WhichKeyStages.Query());

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
        var result = await fixture.SendAsync(new WhichKeyStages.Query
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
        var query = new WhichKeyStages.Query
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
        var result = await fixture.GetPage<WhichKeyStages>().Execute(page =>
        {
            return page.OnGetSubmit(new WhichKeyStages.Command
            {
                Postcode = "AB00BA",
                Subjects = new[]
                {
                    $"{KeyStage.KeyStage1}-English", $"{KeyStage.KeyStage1}-Humanities",
                },
            });
        });

        var resultPage = result.Should().BeAssignableTo<RedirectToPageResult>().Which;
        resultPage.PageName.Should().Be(nameof(WhichSubjects));
        resultPage.RouteValues.Should().ContainKey("Postcode")
            .WhoseValue.Should().Be("AB00BA");
        resultPage.RouteValues.Should().ContainKey("Subjects")
            .WhoseValue.Should().BeEquivalentTo(new[]
            {
                $"{KeyStage.KeyStage1 }-English", $"{KeyStage.KeyStage1 }-Humanities",
            });
    }
}
