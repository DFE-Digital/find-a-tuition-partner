using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Text.Json;

namespace UI.Extensions;

public static class TempDataSerialisation
{
    public static void Set<T>(this ITempDataDictionary tempData, string key, T value) where T : class
    {
        tempData[key] = JsonSerializer.Serialize(value);
    }

    public static T? Get<T>(this ITempDataDictionary tempData, string key) where T : class
    {
        tempData.TryGetValue(key, out var data);
        return data is string json ? JsonSerializer.Deserialize<T>(json) : null;
    }
}