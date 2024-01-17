using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace CookieOidcTokenServiceApi.Client.Services;

public static class CurrentUserExtensions
{
    public static IServiceCollection AddCurrentUser(this IServiceCollection services)
    {
        services.AddScoped<CurrentUser>();
        services.AddScoped<IClaimsTransformation, ClaimsTransformation>();
        return services;
    }
    
    private sealed class ClaimsTransformation(CurrentUser currentUser) : IClaimsTransformation
    {
        private readonly CurrentUser currentUser = currentUser;

        public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            currentUser.Principal = principal;
            return Task.FromResult(principal);
        }
    }
}