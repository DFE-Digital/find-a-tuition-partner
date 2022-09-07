using Microsoft.AspNetCore.Mvc;
using UI.Pages;

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

        result
            .Should().HaveCount(1)
            .And.ContainKey(KeyStage.KeyStage1)
            .WhoseValue.Should().BeEquivalentTo(new[]
            {
                new { Name = "Maths" },
                new { Name = "English" },
                new { Name = "Science" },
            });
    }

    [Fact]
    public async Task Displays_all_subjects_in_key_stage_after_validation_failure()
    {
        var command = new WhichSubjects.Command
        {
            KeyStages = new[] { KeyStage.KeyStage1 }
        };

        var query = new WhichSubjects.Query(command);

        var result = await Fixture.SendAsync(query);

        result
            .Should().HaveCount(1)
            .And.ContainKey(KeyStage.KeyStage1)
            .WhoseValue.Should().BeEquivalentTo(new[]
            {
                new { Name = "Maths" },
                new { Name = "English" },
                new { Name = "Science" },
            });
    }

    [Fact]
    public async Task Displays_all_subjects_when_no_key_stages_selected()
    {
        var result = await Fixture.SendAsync(new WhichSubjects.Query());

        result.Should().ContainKey(KeyStage.KeyStage1)
            .WhoseValue.Should().BeEquivalentTo(new[]
            {
                new { Name = "Maths" },
                new { Name = "English" },
                new { Name = "Science" },
            });

        result.Should().ContainKey(KeyStage.KeyStage2)
            .WhoseValue.Should().BeEquivalentTo(new[]
            {
                new { Name = "Maths" },
                new { Name = "English" },
                new { Name = "Science" },
            });

        result.Should().ContainKey(KeyStage.KeyStage3)
            .WhoseValue.Should().BeEquivalentTo(new[]
            {
                new { Name = "Maths" },
                new { Name = "English" },
                new { Name = "Science" },
                new { Name = "Humanities" },
                new { Name = "Modern foreign languages" },
            });

        result.Should().ContainKey(KeyStage.KeyStage4)
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
            Subjects = new[] { $"{KeyStage.KeyStage1}-English" },
        });

        result.Should().ContainKey(KeyStage.KeyStage1)
            .WhoseValue.Should().BeEquivalentTo(new[]
            {
                new { Name = "English", Selected = true },
                new { Name = "Maths", Selected = false },
                new { Name = "Science", Selected = false },
            });
    }

    [Fact]
    public async Task Updates_selection()
    {
        var result = await Fixture.GetPage<WhichSubjects>().Execute(page =>
        {
            var command = new WhichSubjects.Command
            {
                Subjects = new[]
                {
                    $"{KeyStage.KeyStage1}-English", $"{KeyStage.KeyStage3}-Humanities",
                }
            };
            return page.OnGetSubmit(command);
        });

        var resultPage = result.Should().BeAssignableTo<RedirectToPageResult>().Which;
        resultPage.PageName.Should().Be("SearchResults");
        resultPage.RouteValues.Should().ContainKey("Subjects")
            .WhoseValue.Should().BeEquivalentTo(new[]
            {
                "KeyStage1-English", "KeyStage3-Humanities",
            });
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("This is not a key stage subject")]
    public void Parses_invalid_KeyStageSubject_without_error(string keyStageSubject)
    {
        var parsed = new[] { keyStageSubject }.ParseKeyStageSubjects();
        parsed.Should().BeEmpty();
    }
}