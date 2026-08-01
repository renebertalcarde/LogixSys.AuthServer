using LogixSys.AuthServer.Api.Models;
using LSAuth = LogixSys.AuthServer.Application.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace LogixSys.AuthServer.Api.Controllers;

public class AccountController : Controller
{
    private readonly LSAuth.IAuthenticationService _authenticationService;


    public AccountController(
        LSAuth.IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(
        LoginViewModel model,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _authenticationService.AuthenticateAsync(
            model.UserName,
            model.Password,
            cancellationToken);

        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, "Invalid username or password.");
            return View(model);
        }

        var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, result.UserId!),
        new Claim(ClaimTypes.Name, result.UserName!)
    };

        foreach (var role in result.Roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var identity = new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme);

        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal);

        if (!string.IsNullOrWhiteSpace(model.ReturnUrl) &&
            Url.IsLocalUrl(model.ReturnUrl))
        {
            return Redirect(model.ReturnUrl);
        }

        return Redirect("/");
    }
}