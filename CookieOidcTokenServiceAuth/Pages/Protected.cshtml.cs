using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CookieOidcTokenServiceAuth.Pages;

[Authorize]
public class Protected : PageModel
{
    public void OnGet()
    {
        
    }
}