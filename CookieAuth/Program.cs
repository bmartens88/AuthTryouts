using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

services.AddRazorPages();

// Auth configuration
services.AddAuthentication()
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
        options.Cookie.SameSite = SameSiteMode.Strict;
    });

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();