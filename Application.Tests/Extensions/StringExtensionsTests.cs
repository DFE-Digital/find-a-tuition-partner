using System;
using System.Collections.Generic;
using Application.Extensions;
using Domain.Enums;
using FluentAssertions;
using Xunit;

namespace Application.Tests.Extensions;

public class StringExtensionsTests
{
    [Fact]
    public void ToSeoUrl_ReturnsNull_WhenValueNull()
    {
        string? value = null;

        value.ToSeoUrl().Should().BeNull();
    }

    [Fact]
    public void ToSeoUrl_ReturnsEmptyString_WhenValueEmptyString()
    {
        const string value = "";

        value.ToSeoUrl().Should().Be("");
    }

    [Fact]
    public void ToSeoUrl_ReturnsEmptyString_WhenValueWhitespace()
    {
        const string value = "   ";

        value.ToSeoUrl().Should().Be("");
    }

    [Fact]
    public void ToSeoUrl_ReturnsKebabCase_WhenValuePascalCase()
    {
        const string value = "FindATuitionPartner";

        value.ToSeoUrl().Should().Be("find-a-tuition-partner");
    }

    [Fact]
    public void ToSeoUrl_ReturnsTrimmedKebabCase_WhenValueContainsWhitespace()
    {
        const string value = " FindATuitionPartner ";

        value.ToSeoUrl().Should().Be("find-a-tuition-partner");
    }

    [Fact]
    public void ToSeoUrl_ReturnsKebabCaseWithoutSplitting_WhenValueContainsUppercase()
    {
        const string value = "FindAnNTPApprovedTuitionPartner";

        value.ToSeoUrl().Should().Be("find-an-ntp-approved-tuition-partner");
    }

    [Fact]
    public void ToSeoUrl_ReturnsKebabCase_WhenValueKebabCase()
    {
        const string value = "find-a-tuition-partner";

        value.ToSeoUrl().Should().Be("find-a-tuition-partner");
    }

    [Fact]
    public void ToSeoUrl_ReturnsKebabCase_WhenValueCamelCase()
    {
        const string value = "searchId";

        value.ToSeoUrl().Should().Be("search-id");
    }

    [Fact]
    public void ToSeoUrl_ReturnsKebabCase_WhenHasUnderscore()
    {
        const string value = "search_ Id";

        value.ToSeoUrl().Should().Be("search-id");
    }

    [Theory]
    [InlineData("m2r Education", "m-2-r-education")]
    [InlineData("KeyStage1-Science", "key-stage-1-science")]
    [InlineData("KeyStage1Science", "key-stage-1-science")]
    [InlineData("Keystage1science", "keystage-1-science")]
    [InlineData("KeyStage123Science", "key-stage-123-science")]
    [InlineData("m123r Education", "m-123-r-education")]
    public void ToSeoUrl_ReturnsKebabCase_WhenValueCamelCase_WithNumbers(string camel, string kebab)
    {
        camel.ToSeoUrl().Should().Be(kebab);
    }

    [Fact]
    public void ToSeoUrl_ReturnsKebabCase_WhenValueCamelCase_WithHypen()
    {
        const string value = "PET-Xi Training Limited";

        value.ToSeoUrl().Should().Be("pet-xi-training-limited");
    }

    [Fact]
    public void ToSeoUrl_ReturnsKebabCase_WhenValueCamelCase_WithAmpersand()
    {
        const string value = "CER, Monarch Education & Sugarman Education (Parent- Affinity Workforce Solutions)";

        value.ToSeoUrl().Should().Be("cer-monarch-education-sugarman-education-(parent-affinity-workforce-solutions)");
    }

    [Theory]
    [InlineData("apostrophe's", "apostrophes")]
    [InlineData("special!\"£$%^&*+=chars", "specialchars")]
    [InlineData("more[];:@#,.<>?special", "morespecial")]
    [InlineData("chars-for-aspnet-operation-(){}", "chars-for-aspnet-operation-(){}")]
    [InlineData("unicode६symbols你好", "unicode-symbols")]
    public void ToSeoUrl_ReturnsKebabCase_Without_UrlEncoded_Characters(string name, string seoName)
    {
        name.ToSeoUrl().Should().Be(seoName);
    }

    [Theory]
    [InlineData("Find/Location", "find/location")]
    [InlineData("find/location", "find/location")]
    [InlineData("FindPage/LocationSearch", "find-page/location-search")]
    [InlineData("find-page/location-search", "find-page/location-search")]
    [InlineData("Find/FindAnNTPApprovedTuitionPartner", "find/find-an-ntp-approved-tuition-partner")]
    [InlineData("Find/FindATuitionPartner", "find/find-a-tuition-partner")]
    [InlineData("Find/FindATuitionPartner ", "find/find-a-tuition-partner")]
    [InlineData("Find/findATuitionPartner", "find/find-a-tuition-partner")]
    [InlineData("tuition-partner/A Tuition Co", "tuition-partner/a-tuition-co")]
    public void ToSeoUrl_ReturnsKebabCase_WhenDirectory(string camel, string kebab)
    {
        camel.ToSeoUrl().Should().Be(kebab);
    }

    [Theory]
    [InlineData("KeyStage1-Science", "key-stage-1-science")]
    [InlineData("KeyStage2 Literature", "key-stage-2-literature")]
    [InlineData("KeyStage3 Modern Foreign Languages", "key-stage-3-modern-foreign-languages")]
    public void With_spaces(string value, string expected)
    {
        value.ToSeoUrl().Should().Be(expected);
    }

    [Theory]
    [InlineData("KeyStage1", "key-stage-1")]
    public void With_trailing_digit(string value, string expected)
    {
        value.ToSeoUrl().Should().Be(expected);
    }

    [Fact]
    public void DisplayList_Null()
    {
        IEnumerable<string>? list = null;
        list.DisplayList().Should().Be(string.Empty);
    }

    [Theory]
    [InlineData(new string[] { }, "")]
    [InlineData(new string[] { "test" }, "test")]
    [InlineData(new string[] { "test1 test2" }, "test1 test2")]
    [InlineData(new string[] { "test1", "test2" }, "test1 and test2")]
    [InlineData(new string[] { "test1", "test2", "test3" }, "test1, test2 and test3")]
    public void DisplayList_WithData(string[] stringArray, string output)
    {
        List<string> list = new List<string>(stringArray);
        list.DisplayList().Should().Be(output);
    }

    [Fact]
    public void GroupByKeyAndConcatenateValues_ReturnsCorrectOutput()
    {
        // Arrange
        var keyValuePairs = new List<string>() { "Key1: Value1", "Key2: Value2", "Key2: Value3", "Key3: Value4", "Key3: Value5", "Key3: Value6", "Key3: Value7" };
        var expectedOutput = new List<string>() { "Key1: Value1", "Key2: Value2 and Value3", "Key3: Value4, Value5, Value6 and Value7" };

        // Act
        var actualOutput = keyValuePairs.GroupByKeyAndConcatenateValues();

        // Assert

        actualOutput.Should().BeEquivalentTo(expectedOutput);
    }

    [Theory]
    [InlineData("An error occurred. Status code 400.", 400)]
    [InlineData("An error occurred. Status code 500.", 500)]
    public void GetGovNotifyStatusCodeFromExceptionMessage_ReturnsStatusCode_WhenMatchFound(string errormessage, int expectedStatusCode)
    {
        // Act
        var statusCode = errormessage.GetGovNotifyStatusCodeFromExceptionMessage();

        // Assert

        statusCode.Should().Be(expectedStatusCode);
    }

    [Theory]
    [MemberData(nameof(EscapeNotifyTestData))]
    public void EscapeNotifyText(string text, string expectedResult)
    {
        var result = text.EscapeNotifyText();

        result.Should().Be(expectedResult);
    }

    public static IEnumerable<object[]> EscapeNotifyTestData()
    {
        yield return new object[]
        {
            $"*test line 1{Environment.NewLine}^test line 2{Environment.NewLine}#test line 3{Environment.NewLine}---test line 4{Environment.NewLine}test # line 5",
            $"\\*test line 1{Environment.NewLine}\\^test line 2{Environment.NewLine}\\#test line 3{Environment.NewLine}\\---test line 4{Environment.NewLine}test # line 5"
        };

        yield return new object[]
        {
            $"test line 1{Environment.NewLine}test line 2{Environment.NewLine}test line 3{Environment.NewLine}test line 4{Environment.NewLine}test line 5",
            $"test line 1{Environment.NewLine}test line 2{Environment.NewLine}test line 3{Environment.NewLine}test line 4{Environment.NewLine}test line 5"
        };

        yield return new object[]
        {
            $"* * * test line 1{Environment.NewLine}^ ^ ^test line 2{Environment.NewLine}# # # test line 3{Environment.NewLine}--- --- --------test line 4{Environment.NewLine}test # line 5",
            $"\\* * * test line 1{Environment.NewLine}\\^ ^ ^test line 2{Environment.NewLine}\\# # # test line 3{Environment.NewLine}\\--- --- --------test line 4{Environment.NewLine}test # line 5"
        };

        yield return new object[]
        {
            $"  *test line 1{Environment.NewLine}   ^test line 2{Environment.NewLine}     #test line 3{Environment.NewLine}    ---test line 4{Environment.NewLine}   test # line 5",
            $"\\*test line 1{Environment.NewLine}\\^test line 2{Environment.NewLine}\\#test line 3{Environment.NewLine}\\---test line 4{Environment.NewLine}   test # line 5"
        };

        yield return new object[]
        {
            $"      *test line 1{Environment.NewLine}   ^test line 2{Environment.NewLine}       #test line 3{Environment.NewLine}       ---test line 4{Environment.NewLine}     test # line 5",
            $"\\*test line 1{Environment.NewLine}\\^test line 2{Environment.NewLine}\\#test line 3{Environment.NewLine}\\---test line 4{Environment.NewLine}     test # line 5"
        };
    }

    [Theory]
    [MemberData(nameof(EscapeNotifyTestDataWithInset))]
    public void EscapeNotifyText_WithInset(string text, string expectedResult)
    {
        var result = text.EscapeNotifyText(true);

        result.Should().Be(expectedResult);
    }

    public static IEnumerable<object[]> EscapeNotifyTestDataWithInset()
    {
        yield return new object[]
        {
            $"*test line 1{Environment.NewLine}^test line 2{Environment.NewLine}#test line 3{Environment.NewLine}---test line 4{Environment.NewLine}test # line 5",
            $"\\*test line 1{Environment.NewLine}\\^test line 2{Environment.NewLine}^\\#test line 3{Environment.NewLine}^\\---test line 4{Environment.NewLine}^test # line 5"
        };

        yield return new object[]
        {
            $"test line 1{Environment.NewLine}test line 2{Environment.NewLine}test line 3{Environment.NewLine}test line 4{Environment.NewLine}test line 5",
            $"test line 1{Environment.NewLine}^test line 2{Environment.NewLine}^test line 3{Environment.NewLine}^test line 4{Environment.NewLine}^test line 5"
        };

        yield return new object[]
        {
            $"* * * test line 1{Environment.NewLine}^ ^ ^test line 2{Environment.NewLine}# # # test line 3{Environment.NewLine}--- --- --------test line 4{Environment.NewLine}test # line 5",
            $"\\* * * test line 1{Environment.NewLine}\\^ ^ ^test line 2{Environment.NewLine}^\\# # # test line 3{Environment.NewLine}^\\--- --- --------test line 4{Environment.NewLine}^test # line 5"
        };

        yield return new object[]
        {
            $"  *test line 1{Environment.NewLine}  ^test line 2{Environment.NewLine}  #test line 3{Environment.NewLine}     ---test line 4{Environment.NewLine}  test # line 5",
            $"\\*test line 1{Environment.NewLine}\\^test line 2{Environment.NewLine}^\\#test line 3{Environment.NewLine}^\\---test line 4{Environment.NewLine}^  test # line 5"
        };

        yield return new object[]
        {
            $"      *test line 1{Environment.NewLine}   ^test line 2{Environment.NewLine}       #test line 3{Environment.NewLine}   ---test line 4{Environment.NewLine}     test # line 5",
            $"\\*test line 1{Environment.NewLine}\\^test line 2{Environment.NewLine}^\\#test line 3{Environment.NewLine}^\\---test line 4{Environment.NewLine}^     test # line 5"
        };
    }

    [Fact]
    public void EscapeNotifyText_Empty()
    {
        var result = string.Empty;

        result.Should().BeEmpty();
    }


    [Fact]
    public void EscapeNotifyText_Null()
    {
        string? result = null;

        result.Should().BeNull();
    }

    [Fact]
    public void CreateNotifyClientReference_WithTPName()
    {
        // Arrange
        var enquiryRef = "enquiryRef";
        var tpName = "TP name";

        // Act
        var actualOutput = enquiryRef.CreateNotifyClientReference(EmailTemplateType.EnquirySubmittedConfirmationToEnquirer, tpName);

        // Assert
        actualOutput.Should().BeEquivalentTo($"{enquiryRef}-{EmailTemplateType.EnquirySubmittedConfirmationToEnquirer.DisplayName()}-{tpName.ToSeoUrl()}");
    }

    [Fact]
    public void CreateNotifyClientReference_NoTPName()
    {
        // Arrange
        var enquiryRef = "enquiryRef";

        // Act
        var actualOutput = enquiryRef.CreateNotifyClientReference(EmailTemplateType.EnquirySubmittedConfirmationToEnquirer);

        // Assert
        actualOutput.Should().BeEquivalentTo($"{enquiryRef}-{EmailTemplateType.EnquirySubmittedConfirmationToEnquirer.DisplayName()}");
    }

    [Theory]
    [InlineData("tp-spreadsheets/20230316092358_DfE Tuition Partner.xlsx", "20230316092358_DfE Tuition Partner.xlsx")]
    [InlineData("tp-logos/Logo_1-2-1-test-ltd.svg", "Logo_1-2-1-test-ltd.svg")]
    [InlineData(@"tp-documents\20230414094500_Report.docx", "20230414094500_Report.docx")]
    public void ExtractFileNameFromDirectory_ValidInput_ReturnsCorrectFileName(string inputPath,
        string expectedFileName)
    {
        // Act
        var actualFileName = inputPath.ExtractFileNameFromDirectory();

        // Assert
        Assert.Equal(expectedFileName, actualFileName);
    }

    [Theory]
    [InlineData("ne29 7px", "NE29 7PX")]
    [InlineData("ne297px", "NE29 7PX")]
    [InlineData("ne29      7px", "NE29 7PX")]
    [InlineData("     ne297px", "NE29 7PX")]
    [InlineData("ne297px    ", "NE29 7PX")]
    [InlineData(".. @ /? ne29 ...*&^ ... .. 7px ..£$%..", "NE29 7PX")]
    [InlineData("NE29%207PX", "NE29 7PX")]
    [InlineData("%20N%20E%2029%207%20P%20X%20", "NE29 7PX")]
    [InlineData("ne  29  7  px", "NE29 7PX")]
    [InlineData("ne1 1ad", "NE1 1AD")]
    [InlineData("ne11ad", "NE1 1AD")]
    [InlineData("ne1!1ad", "NE1 1AD")]
    [InlineData(" n e 1 1 a d ", "NE1 1AD")]
    public void ToSanitisedPostcode_Valid(string unsanitisedPostcode, string expectedPostcode)
    {
        // Act
        var sanitisedPostcode = unsanitisedPostcode.ToSanitisedPostcode();

        // Assert
        sanitisedPostcode.Should().Be(expectedPostcode);
    }

    [Theory]
    [InlineData("ne2947px")]
    [InlineData("ne29C7px")]
    [InlineData("ne7px")]
    [InlineData(" ")]
    [InlineData("")]
    public void ToSanitisedPostcode_InValid(string unsanitisedPostcode)
    {
        // Act
        var sanitisedPostcode = unsanitisedPostcode.ToSanitisedPostcode();

        // Assert
        sanitisedPostcode.Should().BeEmpty();
    }

    [Fact]
    public void ToSanitisedPostcode_Null()
    {
        // Arrange
        string? unsanitisedPostcode = null;

        // Act
        var sanitisedPostcode = unsanitisedPostcode.ToSanitisedPostcode();

        // Assert
        sanitisedPostcode.Should().BeNull();
    }
}