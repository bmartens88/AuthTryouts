using System.Security.Claims;

namespace CookieOidcTokenServiceApi.Client.Services;

public sealed class CurrentUser
{
    public ClaimsPrincipal Principal { get; set; } = default!;

    public string Id => Principal.FindFirstValue(ClaimTypes.NameIdentifier)!;
}