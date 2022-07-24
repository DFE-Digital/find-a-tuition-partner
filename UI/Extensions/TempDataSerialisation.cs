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

    public static T? Peek<T>(this ITempDataDictionary? tempData, string key) where T : class
    {
        try
        {
            var data = tempData?.Peek(key);
            if(data is string json) return JsonSerializer.Deserialize<T>(json);
        }
        catch
        {
        }

        return null;
    }
}