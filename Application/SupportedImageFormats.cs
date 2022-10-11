namespace Application;

public static class SupportedImageFormats
{
    private static readonly Dictionary<string, string> ExtensionMimeTypes = new()
    {
        [".svg"] = "image/svg+xml",
        [".png"] = "image/png",
        [".jpg"] = "image/jpeg",
    };

    public static string[] FileExtensions => ExtensionMimeTypes.Keys.ToArray();

    public static string MimeTypeForExtension(string extension)
        => ExtensionMimeTypes.TryGetValue(extension, out var mime)
            ? mime
            : "application/octet-stream";
}