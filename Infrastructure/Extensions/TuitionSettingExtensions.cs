using Application.Extensions;
using TuitionSetting = Domain.Enums.TuitionSetting;

namespace Infrastructure.Extensions
{
    public static class TuitionSettingExtensions
    {
        public static string GetTuitionSettingName(this ICollection<Domain.TuitionSetting>? tuitionSettings)
        {
            if (tuitionSettings != null && tuitionSettings.Any())
            {
                if (tuitionSettings.Count() == 1)
                {
                    var id = tuitionSettings.First().Id;
                    return ((TuitionSetting)id).DisplayName();
                }
                else
                {
                    return TuitionSetting.Both.DisplayName();
                }
            }
            else
            {
                return TuitionSetting.NoPreference.DisplayName();
            }
        }
    }
}
