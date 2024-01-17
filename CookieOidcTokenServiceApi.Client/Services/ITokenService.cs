namespace CookieOidcTokenServiceApi.Client.Services;

public interface ITokenService
{
    void SetUserToken(TokenCacheEntry token);
    TokenCacheEntry? GetUserToken(string? userId);
}