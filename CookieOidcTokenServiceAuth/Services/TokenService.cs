namespace CookieOidcTokenServiceAuth.Services;

public sealed class TokenService : ITokenService
{
    private readonly Dictionary<string, TokenCacheEntry> _entries = [];

    public void SetUserToken(TokenCacheEntry token)
    {
        _entries.Add(token.UserId, token);
    }

    public TokenCacheEntry? GetUserToken(string? userId)
    {
        if (userId is null) return null;

        var entry = _entries[userId];

        return entry;
    }
}