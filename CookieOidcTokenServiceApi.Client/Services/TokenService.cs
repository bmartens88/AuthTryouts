namespace CookieOidcTokenServiceApi.Client.Services;

public sealed class TokenService : ITokenService
{
    private readonly Dictionary<string, TokenCacheEntry> _entries = [];

    public void SetUserToken(TokenCacheEntry token)
    {
        _entries.Add(token.UserId, token);
    }

    public TokenCacheEntry? GetUserToken(string? userId)
    {
        return userId is null ? null : _entries.GetValueOrDefault(userId);
    }
}