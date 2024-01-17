using System.Net.Http.Headers;
using CookieOidcTokenServiceApi.Client.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CookieOidcTokenServiceApi.Client.Pages;

[Authorize]
public class Message(
    CurrentUser currentUser,
    ITokenService tokenService) : PageModel
{
    private readonly CurrentUser currentUser = currentUser;
    private readonly ITokenService tokenService = tokenService;

    public string ResponseMessage { get; private set; } = default!;

    public async Task<IActionResult> OnGetAsync()
    {
        var token = tokenService.GetUserToken(currentUser.Id);
        if (token is null)
            return LocalRedirect("/signout");
        var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
        var response = await client.GetAsync("https://localhost:7229/protected");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        ResponseMessage = content;
        return Page();
    }
}