namespace Application.Extensions;
using TuitionType = Domain.Enums.TuitionType;

public static class IntExtensions
{
    public static string GetTuitionTypeName(this int? tuitionTypeId)
    {
        return tuitionTypeId switch
        {
            null => TuitionType.Any.DisplayName(),
            (int)TuitionType.Online => TuitionType.Online.DisplayName(),
            (int)TuitionType.InSchool => TuitionType.InSchool.DisplayName(),
            _ => TuitionType.Any.DisplayName()
        };
    }
}