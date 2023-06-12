using System.ComponentModel;
using System.Reflection;
using Domain.Attributes;

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
        Dictionary<string, int> orderTable = new();

        var enumValues = Enum.GetValues(typeof(TEnum)).Cast<TEnum>().ToList();

        var members = typeof(TEnum).GetMembers();

        foreach (MemberInfo member in members)
        {
            var attributes = member.GetCustomAttributes(typeof(OrderAttribute), false);

            foreach (object attribute in attributes)
            {
                if (attribute is OrderAttribute orderAttribute)
                {
                    orderTable.Add(member.Name, orderAttribute.Order);
                }
            }
        }

        if (orderTable.Count == enumValues.Count)
        {
            enumValues = enumValues.OrderBy(n => orderTable[n.ToString("G")]).ToList();
        }

        return enumValues;
    }

    private static DescriptionAttribute? GetDisplayAttribute(Enum enumValue)
        => enumValue
            .GetType()
            .GetMember(enumValue.ToString())
            .FirstOrDefault()
            ?.GetCustomAttribute<DescriptionAttribute>();
}