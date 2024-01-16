namespace CookieOidcTokenServiceAuth.Services;

public sealed class TokenCacheEntry
{
    public string UserId { get; init; }
    public string AccessToken { get; init; }
    public string RefreshToken { get; init; }
    public DateTimeOffset ExpiresUtc { get; init; }
}