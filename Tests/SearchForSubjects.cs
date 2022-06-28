using Domain;
using Microsoft.AspNetCore.Mvc;
using UI.Pages.FindATuitionPartner;

namespace Tests;

[Collection(nameof(SliceFixture))]
public class SearchForSubjects : CleanSliceFixture
{
    public SearchForSubjects(SliceFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public async Task Displays_all_subjects_in_database()
    {
        await Fixture.InsertAsync(
            new Subject { Name = "English" },
            new Subject { Name = "Maths" },
            new Subject { Name = "Science" }
            );

        var result = await Fixture.SendAsync(new Subjects.Query());

        result.AllSubjects.Should().BeEquivalentTo(new[]
        {
            new { Name = "English" },
            new { Name = "Maths" },
            new { Name = "Science" },
        });
    }

    [Fact]
    public async Task Preserves_selected_from_querystring()
    {
        await Fixture.InsertAsync(
            new Subject { Name = "English" },
            new Subject { Name = "Maths" },
            new Subject { Name = "Science" }
            );

        var result = await Fixture.SendAsync(new Subjects.Query
        {
            Subjects = new[] { "English" }
        });

        result.AllSubjects.Should().BeEquivalentTo(new[]
        {
            new { Name = "English", Selected = true },
            new { Name = "Maths", Selected = false },
            new { Name = "Science", Selected = false },
        });
    }

    [Fact]
    public async Task Preserves_other_search_parameters()
    {
        var query = new Subjects.Query
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
        var result = await Fixture.GetPage<Subjects>().Execute(page =>
        {
            page.Data.Subjects = new List<string>
            {
                "English", "Humanities",
            };
            return page.OnPost();
        });

        var resultPage = result.Should().BeAssignableTo<RedirectToPageResult>().Which;
        resultPage.PageName.Should().Be("Results");
        resultPage.RouteValues.Should().ContainKey("Subjects")
            .WhoseValue.Should().BeEquivalentTo(new[]
            {
                "English", "Humanities",
            });
    }
}