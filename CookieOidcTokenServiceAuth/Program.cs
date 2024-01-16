using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using CookieOidcTokenServiceAuth.Services;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

services.AddRazorPages();

// Register token service for access- and refresh-token storage.
services.AddSingleton<ITokenService, TokenService>();

services.AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
    })
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
        options.ExpireTimeSpan = TimeSpan.FromDays(1);
        options.Events = new CookieAuthenticationEvents
        {
            OnValidatePrincipal = async x =>
            {
                // Since cookie lifetime is based on the access token one,
                // check if we're more than halfway of the cookie lifetime
                var now = DateTimeOffset.UtcNow;
                var timeElapsed = now.Subtract(x.Properties.IssuedUtc!.Value);
                var timeRemaining = x.Properties.ExpiresUtc!.Value.Subtract(now);

                if (timeElapsed > timeRemaining)
                {
                    var userId = x.Principal!.FindFirst(ClaimTypes.NameIdentifier)!.Value;
                    var cache = x.HttpContext.RequestServices.GetRequiredService<ITokenService>();
                    var actualToken = cache.GetUserToken(userId);

                    if (actualToken is null)
                        return;

                    // Refresh token from OIDC endpoint
                    var response = await new HttpClient().RequestRefreshTokenAsync(new RefreshTokenRequest
                    {
                        Address = "https://demo.duendesoftware.com/connect/token",
                        ClientId = "interactive.confidential",
                        ClientSecret = "secret",
                        RefreshToken = actualToken.RefreshToken,
                        GrantType = "refresh_token"
                    });

                    if (!response.IsError)
                    {
                        // Update access-token in service
                        cache.SetUserToken(new TokenCacheEntry
                        {
                            UserId = userId,
                            RefreshToken = response.RefreshToken!,
                            AccessToken = response.AccessToken!,
                            ExpiresUtc = new JwtSecurityToken(response.AccessToken).ValidTo
                        });

                        // Tell cookie middleware to renew the session cookie
                        x.ShouldRenew = true;
                    }
                }
            }
        };
    })
    .AddOpenIdConnect(options =>
    {
        options.Authority = "https://demo.duendesoftware.com";
        options.ClientId = "interactive.confidential";
        options.ClientSecret = "secret";
        options.ResponseType = OpenIdConnectResponseType.Code;
        options.SaveTokens = false; // This is the default, but to be explicit
        options.GetClaimsFromUserInfoEndpoint = true;

        options.Events = new OpenIdConnectEvents
        {
            OnTokenValidated = x =>
            {
                // Extract access- and refresh-token and store in service
                var cache = x.HttpContext.RequestServices.GetRequiredService<ITokenService>();
                var token = new TokenCacheEntry
                {
                    UserId = x.Principal!.FindFirst(ClaimTypes.NameIdentifier)!.Value,
                    AccessToken = x.TokenEndpointResponse!.AccessToken,
                    RefreshToken = x.TokenEndpointResponse.RefreshToken,
                    ExpiresUtc = new JwtSecurityToken(x.TokenEndpointResponse.AccessToken).ValidTo
                };
                x.Properties!.IsPersistent = true;
                x.Properties.ExpiresUtc = new JwtSecurityToken(x.TokenEndpointResponse.AccessToken).ValidTo;

                cache.SetUserToken(token);

                return Task.CompletedTask;
            }
        };
    });

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();