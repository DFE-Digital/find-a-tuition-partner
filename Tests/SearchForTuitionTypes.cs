using Domain.Enums;
using FluentValidation.TestHelper;
using UI.Pages;

namespace Tests;

[Collection(nameof(SliceFixture))]
public class SearchForTuitionTypes : CleanSliceFixture
{
    public SearchForTuitionTypes(SliceFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public void With_an_empty_tuition_type()
    {
        var model = new WhichTuitionTypes.Command { TuitionType = null };
        var result = new WhichTuitionTypes.Validator().TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.TuitionType)
            .WithErrorMessage("Select a type of tuition");
    }

    [Fact]
    public async Task Updates_selection()
    {
        var result = await Fixture.GetPage<WhichTuitionTypes>().Execute(page =>
        {
            var command = new WhichTuitionTypes.Command
            {
                TuitionType = TuitionType.InSchool
            };
            return page.OnGetSubmit(command);
        });

        var resultPage = result.Should().BeAssignableTo<RedirectToPageResult>().Which;
        resultPage.PageName.Should().Be("SearchResults");
        resultPage.RouteValues.Should().ContainKey("TuitionType")
            .WhoseValue.Should().Be(TuitionType.InSchool);
    }
}