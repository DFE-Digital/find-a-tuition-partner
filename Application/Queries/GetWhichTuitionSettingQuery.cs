using Application.Common.Models;

namespace Application.Queries;

public record GetWhichTuitionSettingQuery : SearchModel
{
    public GetWhichTuitionSettingQuery() { }
    public GetWhichTuitionSettingQuery(SearchModel query) : base(query) { }
}