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

    private static DescriptionAttribute? GetDisplayAttribute(Enum enumValue)
        => enumValue
            .GetType()
            .GetMember(enumValue.ToString())
            .FirstOrDefault()
            ?.GetCustomAttribute<DescriptionAttribute>();
}