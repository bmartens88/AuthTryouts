using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using CookieOidcTokenServiceApi.Client.Services;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

services.AddSingleton<ITokenService, TokenService>();
services.AddCurrentUser();

services.AddRazorPages();

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
                var now = DateTimeOffset.UtcNow;
                var timeElapsed = now.Subtract(x.Properties.IssuedUtc!.Value);
                var timeRemaining = x.Properties.ExpiresUtc!.Value.Subtract(now);

                if (timeElapsed > timeRemaining)
                {
                    var userId = x.Principal!.FindFirstValue(ClaimTypes.NameIdentifier)!;
                    var cache = x.HttpContext.RequestServices.GetRequiredService<ITokenService>();
                    var actualToken = cache.GetUserToken(userId);

                    if (actualToken is null)
                        return;

                    var response = await new HttpClient().RequestRefreshTokenAsync(new RefreshTokenRequest
                    {
                        Address = "https://demo.duendesoftware.com/connect/token",
                        ClientId = "interactive.confidential",
                        ClientSecret = "secret",
                        RefreshToken = actualToken.RefreshToken,
                        GrantType = OpenIdConnectGrantTypes.RefreshToken
                    });

                    if (!response.IsError)
                    {
                        cache.SetUserToken(new TokenCacheEntry
                        {
                            UserId = userId,
                            RefreshToken = response.RefreshToken!,
                            AccessToken = response.AccessToken!,
                            ExpiresUtc = new JwtSecurityToken(response.AccessToken).ValidTo
                        });

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
        options.SaveTokens = false;
        options.GetClaimsFromUserInfoEndpoint = true;
        options.Scope.Add("offline_access"); // To include refresh token

        options.Events = new OpenIdConnectEvents
        {
            OnTokenValidated = x =>
            {
                var cache = x.HttpContext.RequestServices.GetRequiredService<ITokenService>();
                var token = new TokenCacheEntry
                {
                    UserId = x.Principal!.FindFirstValue(ClaimTypes.NameIdentifier)!,
                    AccessToken = x.TokenEndpointResponse!.AccessToken,
                    RefreshToken = x.TokenEndpointResponse.RefreshToken,
                    ExpiresUtc = new JwtSecurityToken(x.TokenEndpointResponse.AccessToken).ValidTo
                };
                x.Properties!.IsPersistent = true;
                x.Properties!.ExpiresUtc = new JwtSecurityToken(x.TokenEndpointResponse.AccessToken).ValidTo;

                cache.SetUserToken(token);

                return Task.CompletedTask;
            }
        };
    });

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();