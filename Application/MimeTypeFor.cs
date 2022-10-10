namespace Application;

public static class MimeTypeFor
{
    public static string Extension(string extension)
        => extension switch
        {
            ".svg" => "image/svg+xml",
            ".png" => "image/png",
            ".jpg" => "image/jpeg",
            _ => "application/octet-stream"
        };
}