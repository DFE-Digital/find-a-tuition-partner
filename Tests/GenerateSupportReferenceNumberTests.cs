using Infrastructure.Services;

namespace Tests;

public class GenerateSupportReferenceNumberTests
{
    private readonly GenerateSupportReferenceNumber _generateSupportReferenceNumber;

    public GenerateSupportReferenceNumberTests()
    {
        _generateSupportReferenceNumber = new GenerateSupportReferenceNumber();
    }

    [Fact]
    public void GenerateReferenceNumber_ShouldReturn_StringWithCorrectLength()
    {
        // Arrange
        const int numberOfLetters = 2;
        const int numberOfDigits = 4;

        // Act
        var referenceNumber = _generateSupportReferenceNumber.GenerateReferenceNumber();

        // Assert
        referenceNumber.Should().HaveLength(numberOfLetters + numberOfDigits);
    }

    [Fact]
    public void GenerateReferenceNumber_ShouldNotContain_UnclearLetters()
    {
        // Arrange
        char[] unclearLetters = { 'I', 'L', 'O' };

        // Act
        var referenceNumber = _generateSupportReferenceNumber.GenerateReferenceNumber();

        // Assert
        referenceNumber.Should().NotContain(string.Join(",", unclearLetters.Select(c => c.ToString())));
    }

    [Fact]
    public void GenerateReferenceNumber_ShouldNotContain_UnclearDigits()
    {
        // Arrange
        int[] unclearDigits = { 0, 1 };

        // Act
        var referenceNumber = _generateSupportReferenceNumber.GenerateReferenceNumber();

        // Assert
        referenceNumber.Should().NotContain(string.Join("", unclearDigits));
    }

    [Fact]
    public void GenerateReferenceNumber_ShouldContain_LettersAndDigits()
    {
        // Arrange

        // Act
        var referenceNumber = _generateSupportReferenceNumber.GenerateReferenceNumber();

        // Assert
        referenceNumber.Should().MatchRegex("^[A-HJ-KM-NP-Z]{2}[2-9]{4}$");
    }
}
