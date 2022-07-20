using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Text.Json;

namespace UI.Extensions;

public static class TempDataSerialisation
{
    public static void Set<T>(this ITempDataDictionary? tempData, string key, T value) where T : class
    {
        if (tempData != null)
            tempData[key] = JsonSerializer.Serialize(value);
    }

    public static T? Get<T>(this ITempDataDictionary? tempData, string key) where T : class
    {
        if (tempData == null) return null;

        try
        {
            tempData.TryGetValue(key, out var data);
            return data is string json ? JsonSerializer.Deserialize<T>(json) : null;
        }
        catch
        {
            return null;
        }
    }
}