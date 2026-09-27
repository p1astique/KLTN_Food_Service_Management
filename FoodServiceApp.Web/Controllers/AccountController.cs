using System.Security.Claims;
using FoodServiceApp.Web.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodServiceApp.Web.Controllers;

public sealed class AccountController(VendorDataStore store, IConfiguration configuration) : Controller
{
    [AllowAnonymous]
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true) return RedirectToAction("Index", "Dashboard");
        ViewBag.ReturnUrl = returnUrl;
        return View();
    }

    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(string email, string password, string? returnUrl = null)
    {
        var expectedEmail = configuration["DemoAccount:Email"] ?? "comtam.saigon@gianhang.vn";
        var expectedPassword = configuration["DemoAccount:Password"];
        if (string.IsNullOrWhiteSpace(expectedPassword))
        {
            ViewBag.Error = "Chưa cấu hình mật khẩu demo. Xem hướng dẫn trong README của project web.";
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
        if (!string.Equals(email?.Trim(), expectedEmail, StringComparison.OrdinalIgnoreCase) || password != expectedPassword)
        {
            ViewBag.Error = "Email hoặc mật khẩu chưa đúng.";
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        var profile = store.Read(d => d.Store);
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, profile.Id),
            new(ClaimTypes.Name, profile.Name),
            new(ClaimTypes.Email, profile.Email)
        };
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
        return !string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl)
            ? Redirect(returnUrl)
            : RedirectToAction("Index", "Dashboard");
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction(nameof(Login));
    }
}
