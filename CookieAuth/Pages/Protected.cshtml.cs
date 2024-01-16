using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CookieAuth.Pages;

[Authorize]
public class Protected : PageModel
{
    public void OnGet()
    {
        
    }
}