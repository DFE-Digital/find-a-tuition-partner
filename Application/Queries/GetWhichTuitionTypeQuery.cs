using Application.Common.Models;

namespace Application.Queries;

public record GetWhichTuitionTypeQuery : SearchModel
{
    public GetWhichTuitionTypeQuery() { }
    public GetWhichTuitionTypeQuery(SearchModel query) : base(query) { }
}