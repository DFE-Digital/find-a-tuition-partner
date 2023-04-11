namespace Application.Common.DTO;

public record DriveItem
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public DateTimeOffset CreatedDateTime { get; set; }
    public DateTimeOffset LastModifiedDateTime { get; set; }
    public string? ETag { get; set; }
    public int Size { get; set; }
}