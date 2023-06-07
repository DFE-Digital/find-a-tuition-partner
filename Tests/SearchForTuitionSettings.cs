using Domain.Enums;
using FluentValidation.TestHelper;
using UI.Pages;

namespace Tests;

[Collection(nameof(SliceFixture))]
public class SearchForTuitionSettings : CleanSliceFixture
{
    public SearchForTuitionSettings(SliceFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public void With_an_empty_tuition_setting()
    {
        var model = new WhichTuitionSettings.Command { TuitionSetting = null };
        var result = new WhichTuitionSettings.Validator().TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.TuitionSetting)
            .WithErrorMessage("Select a tuition setting");
    }

    [Fact]
    public async Task Updates_selection()
    {
        var result = await Fixture.GetPage<WhichTuitionSettings>().Execute(page =>
        {
            var command = new WhichTuitionSettings.Command
            {
                TuitionSetting = TuitionSetting.FaceToFace
            };
            return page.OnGetSubmit(command);
        });

        var resultPage = result.Should().BeAssignableTo<RedirectToPageResult>().Which;
        resultPage.PageName.Should().Be("SearchResults");
        resultPage.RouteValues.Should().ContainKey("TuitionSetting")
            .WhoseValue.Should().Be(TuitionSetting.FaceToFace);
    }
}