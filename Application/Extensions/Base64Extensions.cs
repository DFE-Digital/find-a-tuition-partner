using System.Text;

namespace Application.Extensions;

public static class Base64Extensions
{
    public static string ToBase64Filename(this string value)
    {
        return Encoding.UTF8.GetBytes(value).ToBase64Filename();
    }

    public static string ToBase64Filename(this byte[] bytes)
    {
        var base64 = Convert.ToBase64String(bytes);
        var noPlus = base64.Replace('+', '-');
        var noForwardSlash = noPlus.Replace('/', '_');
        return noForwardSlash;
    }

    public static byte[] FromBase64Filename(this string base64Filename)
    {
        var withForwardSlash = base64Filename.Replace('_', '/');
        var withPlus = withForwardSlash.Replace('-', '+');
        var bytes = Convert.FromBase64String(withPlus);
        return bytes;
    }
}