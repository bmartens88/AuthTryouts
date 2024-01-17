using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

services.AddAuthorization();
services.AddAuthentication()
    .AddJwtBearer(options =>
    {
        options.Authority = "https://demo.duendesoftware.com";
        
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = "https://demo.duendesoftware.com",
            // ValidateLifetime = true,
            ValidateAudience = false // Duende demo server doesn't include aud claim
        };
    });

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/protected", () => TypedResults.Ok("Hello from protected endpoint!"))
    .RequireAuthorization();

app.Run();