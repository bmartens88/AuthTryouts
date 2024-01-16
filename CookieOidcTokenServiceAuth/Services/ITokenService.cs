namespace CookieOidcTokenServiceAuth.Services;

public interface ITokenService
{
    void SetUserToken(TokenCacheEntry token);
    TokenCacheEntry? GetUserToken(string? userId);
}