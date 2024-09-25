namespace MSALDemo;

public static class UriExtensions
{
    public static string? GetToken(this Uri uri, Uri redirectUri)
    {
        var urlParts = uri.AbsoluteUri?.Split('&');
        if (urlParts?.Length > 0)
        {
            int prefPart = $"{redirectUri}?code=".Length;
            string token = urlParts[0].Substring(prefPart);
            return token;
        }
        return null;
    }
}
