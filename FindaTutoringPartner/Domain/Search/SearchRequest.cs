using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Domain.Search;

public abstract class SearchRequestBase
{
    public const int DefaultPageSize = 50;
    
    [Range(0, int.MaxValue)]
    [DefaultValue(0)]
    public int Page { get; set; }
    [Range(1, 500)]
    [DefaultValue(DefaultPageSize)]
    public int PageSize { get; set; } = DefaultPageSize;
}