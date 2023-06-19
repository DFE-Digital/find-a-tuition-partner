using Application.Common.Models.Enquiry;
using Newtonsoft.Json;

namespace Application.Extensions;

public static class TutoringLogisticsDetailsModelExtensions
{
    public static string ToJson(this TutoringLogisticsDetailsModel tutoringLogisticsDetailsModel)
    {
        return JsonConvert.SerializeObject(tutoringLogisticsDetailsModel);
    }
}