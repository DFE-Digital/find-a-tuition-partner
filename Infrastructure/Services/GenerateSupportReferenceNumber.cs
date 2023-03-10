using Application.Common.Interfaces;

namespace Infrastructure.Services;

public class GenerateSupportReferenceNumber : IGenerateReferenceNumber
{
    public string GenerateReferenceNumber()
    {
        const int numberOfLetters = 2;
        const int numberOfDigits = 4;

        char[] unclearLetters =
        {
            'I', 'L', 'O'
        };
        int[] unclearDigits =
        {
            0, 1
        };

        var letters = Enumerable.Range('A', 26).Select(x => (char)x).Where(x => !unclearLetters.Contains(x)).ToList();
        var digits = Enumerable.Range('1', 9).Select(x => (char)x).Where(x => !unclearDigits.Contains(int.Parse(x.ToString()))).ToList();

        var random = new Random();
        var randomLetters = Enumerable.Range(1, numberOfLetters).Select(x => letters[random.Next(letters.Count)]).ToList();
        var randomDigits = Enumerable.Range(1, numberOfDigits).Select(x => digits[random.Next(digits.Count)]).ToList();

        return new string(randomLetters.Concat(randomDigits).ToArray());
    }
}