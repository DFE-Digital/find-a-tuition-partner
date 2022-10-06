using Infrastructure;

namespace Tests.Importing;

public class ImportLogoFiles
{
    [Theory]
    [InlineData("seo-name", "Logo_seo-name.svg")]
    [InlineData("tuition-partner", "Logo_tuition-partner.png")]
    public void Logo_file_matches_seo_name_exactly(string name, string logoFile)
    {
        DataImporterService.IsFileLogoForTuitionPartner(name, logoFile).Should().BeTrue();
    }

    [Theory]
    [InlineData("seo-name", "Logo_banana.svg")]
    [InlineData("seo-name", "Logo_other-seo-name.svg")]
    [InlineData("seo-name", "Logo_seo-name_logo.svg")]
    [InlineData("seo-name", "seo-name.svg")]
    public void Logo_file_does_not_match_seo_name_exactly(string name, string logoFile)
    {
        DataImporterService.IsFileLogoForTuitionPartner(name, logoFile).Should().BeFalse();
    }
}