using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CookieOidcAuth.Pages;

[AllowAnonymous]
public class Login(IAuthenticationSchemeProvider authenticationSchemeProvider) : PageModel
{
    private readonly IAuthenticationSchemeProvider authenticationSchemeProvider = authenticationSchemeProvider;

    [BindProperty] public LoginModel LoginModel { get; set; } = new();

    public string? ReturnUrl { get; set; }

    public IList<AuthenticationScheme>? ExternalProviders { get; set; }

    public async Task OnGetAsync(string? returnUrl)
    {
        returnUrl ??= Url.Content("~/");
        ReturnUrl = returnUrl;

        ExternalProviders = (await GetExternalProviders()).ToList();
    }

    public async Task<IActionResult> OnPostAsync(string? returnUrl)
    {
        returnUrl ??= Url.Content("~/");
        if (!ModelState.IsValid)
            return Page();
        if (LoginModel.Username != "bas" || LoginModel.Password != "secret") return Page();
        IEnumerable<Claim> claims =
        [
            new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Name, LoginModel.Username),
            new Claim(ClaimTypes.Email, "test@test.com")
        ];
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);
        await HttpContext.SignInAsync(principal);

        return LocalRedirect(returnUrl);
    }

    public IActionResult OnPostOidc(string? returnUrl)
    {
        return Challenge(new AuthenticationProperties
        {
            RedirectUri = !string.IsNullOrEmpty(returnUrl) ? returnUrl : "/"
        }, OpenIdConnectDefaults.AuthenticationScheme);
    }

    private async Task<IEnumerable<AuthenticationScheme>> GetExternalProviders()
    {
        var schemes = await authenticationSchemeProvider.GetAllSchemesAsync();
        return schemes.Where(s => !string.IsNullOrEmpty(s.DisplayName));
    }
}

public class LoginModel
{
    [Required] [StringLength(10)] public string? Username { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string? Password { get; set; }

    public string? ReturnUrl { get; set; }
}