using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CookieOidcTokenServiceApi.Client.Pages;

public class Index : PageModel
{
    public string Message { get; private set; } = "PageModel in C#";
    public void OnGet()
    {
        Message += $" Server time is {DateTime.Now}";
    }
}