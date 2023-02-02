using System.ComponentModel;
using System.Reflection;

namespace Application.Extensions;

public static class EnumExtensions
{
    public static string DisplayName(this Enum enumValue)
    {
        var displayName =
            GetDisplayAttribute(enumValue)
            ?.Description;

        return string.IsNullOrEmpty(displayName)
            ? enumValue.ToString()
            : displayName;
    }

    public static List<TEnum> GetAllEnums<TEnum>()
        where TEnum : struct, Enum
    {
        return Enum.GetValues(typeof(TEnum)).Cast<TEnum>().ToList();
    }

    public static bool TryParse<TEnum>(this string displayName, out TEnum resultInputType)
        where TEnum : struct, Enum
    {
        foreach (TEnum enumLoop in Enum.GetValues(typeof(TEnum)))
        {
            var displayNameLoop = DisplayName(enumLoop);
            if (displayNameLoop.Equals(displayName, StringComparison.InvariantCultureIgnoreCase))
            {
                resultInputType = enumLoop;
                return true;
            }
        }
        resultInputType = default;
        return false;
    }

    private static DescriptionAttribute? GetDisplayAttribute(Enum enumValue)
        => enumValue
            .GetType()
            .GetMember(enumValue.ToString())
            .FirstOrDefault()
            ?.GetCustomAttribute<DescriptionAttribute>();
}