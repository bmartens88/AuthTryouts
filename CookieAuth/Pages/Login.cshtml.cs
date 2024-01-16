using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CookieAuth.Pages;

[AllowAnonymous]
public class Login : PageModel
{
    [BindProperty] public LoginModel LoginModel { get; set; } = new();

    public string? ReturnUrl { get; set; }

    public void OnGet(string? returnUrl)
    {
        returnUrl ??= Url.Content("~/");
        ReturnUrl = returnUrl;
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
}

public class LoginModel
{
    [Required] [StringLength(10)] public string? Username { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string? Password { get; set; }

    public string? ReturnUrl { get; set; }
}