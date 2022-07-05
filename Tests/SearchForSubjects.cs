using Domain;
using Microsoft.AspNetCore.Mvc;
using UI.Pages.FindATuitionPartner;
using KeyStage = UI.Pages.FindATuitionPartner.KeyStage;

namespace Tests;

[Collection(nameof(SliceFixture))]
public class SearchForSubjects : CleanSliceFixture
{
    public SearchForSubjects(SliceFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public async Task Displays_all_subjects_in_key_stage()
    {
        var result = await Fixture.SendAsync(new WhichSubjects.Query
        {
            KeyStages = new[] { KeyStage.KeyStage1 }
        });

        result.AllSubjects.Should().ContainKey(KeyStage.KeyStage1)
            .WhoseValue.Should().BeEquivalentTo(new[]
            {
                new { Name = "Numeracy" },
                new { Name = "Literacy" },
                new { Name = "Science" },
            });
    }

    [Fact]
    public async Task Displays_all_subjects_when_no_key_stages_selected()
    {
        var result = await Fixture.SendAsync(new WhichSubjects.Query());

        result.AllSubjects.Should().ContainKey(KeyStage.KeyStage1)
            .WhoseValue.Should().BeEquivalentTo(new[]
            {
                new { Name = "Numeracy" },
                new { Name = "Literacy" },
                new { Name = "Science" },
            });

        result.AllSubjects.Should().ContainKey(KeyStage.KeyStage2)
            .WhoseValue.Should().BeEquivalentTo(new[]
            {
                new { Name = "Numeracy" },
                new { Name = "Literacy" },
                new { Name = "Science" },
            });

        result.AllSubjects.Should().ContainKey(KeyStage.KeyStage3)
            .WhoseValue.Should().BeEquivalentTo(new[]
            {
                new { Name = "Maths" },
                new { Name = "English" },
                new { Name = "Science" },
                new { Name = "Humanities" },
                new { Name = "Modern foreign languages" },
            });
    }

    [Fact]
    public async Task Preserves_selected_from_querystring()
    {
        var result = await Fixture.SendAsync(new WhichSubjects.Query
        {
            KeyStages = new[] { KeyStage.KeyStage1 },
            Subjects = new[] { $"{KeyStage.KeyStage1}-Literacy" },
        });

        result.AllSubjects.Should().ContainKey(KeyStage.KeyStage1)
            .WhoseValue.Should().BeEquivalentTo(new[]
            {
                new { Name = "Literacy", Selected = true },
                new { Name = "Numeracy", Selected = false },
                new { Name = "Science", Selected = false },
            });
    }

    [Fact]
    public async Task Preserves_other_search_parameters()
    {
        var query = new WhichSubjects.Query
        {
            Postcode = "123456",
        };

        var result = await Fixture.SendAsync(query);

        result.Should().BeEquivalentTo(new
        {
            Postcode = "123456",
        });
    }

    [Fact]
    public async Task Updates_selection()
    {
        var result = await Fixture.GetPage<WhichSubjects>().Execute(page =>
        {
            page.Data.Subjects = new[]
            {
                $"{KeyStage.KeyStage1}-English", $"{KeyStage.KeyStage3}-Humanities",
            };
            return page.OnPost();
        });

        var resultPage = result.Should().BeAssignableTo<RedirectToPageResult>().Which;
        resultPage.PageName.Should().Be("SearchResults");
        resultPage.RouteValues.Should().ContainKey("Subjects")
            .WhoseValue.Should().BeEquivalentTo(new[]
            {
                "KeyStage1-English", "KeyStage3-Humanities",
            });
    }
}