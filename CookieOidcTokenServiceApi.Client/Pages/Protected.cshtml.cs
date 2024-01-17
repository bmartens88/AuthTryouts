using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CookieOidcTokenServiceApi.Client.Pages;

[Authorize]
public class Protected : PageModel
{
    public void OnGet()
    {
        
    }
}