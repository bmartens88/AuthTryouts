using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CookieOidcAuth.Pages;

[Authorize]
public class Protected : PageModel
{
    public void OnGet()
    {
        
    }
}