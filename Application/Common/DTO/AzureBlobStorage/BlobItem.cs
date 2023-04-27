namespace Application.Common.DTO.AzureBlobStorage;

public record BlobItem
{
    /// <summary>
    /// Name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Deleted.
    /// </summary>
    public bool Deleted { get; set; }

    /// <summary>
    /// Snapshot.
    /// </summary>
    public string? Snapshot { get; set; } = string.Empty;

    /// <summary>
    /// VersionId.
    /// </summary>
    public string? VersionId { get; set; } = string.Empty;

    /// <summary>
    /// IsCurrentVersion.
    /// </summary>
    public bool? IsLatestVersion { get; set; }

    /// <summary>
    /// Metadata.
    /// </summary>
    public IDictionary<string, string>? Metadata { get; set; } = new Dictionary<string, string>();

    /// <summary>
    /// Indicates that this root blob has been deleted, but it has versions that are active.
    /// </summary>
    public bool? HasVersionsOnly { get; set; }
}