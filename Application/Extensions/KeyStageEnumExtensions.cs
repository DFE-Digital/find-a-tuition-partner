using Domain.Enums;

namespace Application.Extensions;

public static class KeyStageEnumExtensions
{
    public static KeyStage[]? UpdateFromSubjects(this KeyStage[]? keyStages, string[]? subjects)
    {
        if (keyStages == null && subjects != null)
        {
            return Enum.GetValues(typeof(KeyStage)).Cast<KeyStage>()
                .Where(x => string.Join(" ", subjects).Contains(x.ToString())).ToArray();
        }
        return keyStages;
    }
}