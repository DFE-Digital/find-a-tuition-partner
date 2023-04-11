namespace Application.Common.DTO;

public record DriveItemCollectionResponse
{
    public List<DriveItem> Value { get; set; } = new List<DriveItem>();
}