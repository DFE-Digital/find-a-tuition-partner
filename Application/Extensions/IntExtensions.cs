namespace Application.Extensions;
using TuitionSetting = Domain.Enums.TuitionSetting;

public static class IntExtensions
{
    public static string GetTuitionSettingName(this int? tuitionSettingId)
    {
        return tuitionSettingId switch
        {
            null => TuitionSetting.NoPreference.DisplayName(),
            (int)TuitionSetting.Online => TuitionSetting.Online.DisplayName(),
            (int)TuitionSetting.FaceToFace => TuitionSetting.FaceToFace.DisplayName(),
            _ => TuitionSetting.NoPreference.DisplayName()
        };
    }

    public static bool Is4xxError(this int statusCode)
    {
        return statusCode >= 400 && statusCode < 500;
    }
}