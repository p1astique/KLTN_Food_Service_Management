using FoodServiceApp.Web.Models;
using FoodServiceApp.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodServiceApp.Web.Controllers;

public sealed class RegistrationController(VendorDataStore store) : Controller
{
    [AllowAnonymous, HttpGet]
    public IActionResult Index() => View(new VendorRegistrationForm());

    [AllowAnonymous, HttpPost, ValidateAntiForgeryToken]
    public IActionResult Index(VendorRegistrationForm input)
    {
        input.OwnerName = input.OwnerName?.Trim() ?? "";
        input.StoreName = input.StoreName?.Trim() ?? "";
        input.Email = input.Email?.Trim() ?? "";
        input.Phone = input.Phone?.Trim() ?? "";
        input.Address = input.Address?.Trim() ?? "";
        input.BusinessType = input.BusinessType?.Trim() ?? "";

        if (input.OwnerName.Length < 2 || input.StoreName.Length < 2 ||
            !input.Email.Contains('@') || input.Phone.Length < 9 ||
            input.Address.Length < 5 || input.BusinessType.Length < 2)
        {
            ViewBag.Error = "Vui lòng nhập đầy đủ và đúng thông tin đăng ký gian hàng.";
            return View(input);
        }

        if (store.Read(d => d.Registrations.Any(r =>
            r.Email.Equals(input.Email, StringComparison.OrdinalIgnoreCase) &&
            r.Status == RegistrationStatus.ChoDuyet)))
        {
            ViewBag.Error = "Email này đã có một hồ sơ đang chờ duyệt.";
            return View(input);
        }

        var id = "DK" + DateTime.Now.ToString("yyyyMMddHHmmss");
        store.Update(d => d.Registrations.Add(new VendorRegistration
        {
            Id = id,
            OwnerName = input.OwnerName,
            StoreName = input.StoreName,
            Email = input.Email,
            Phone = input.Phone,
            Address = input.Address,
            BusinessType = input.BusinessType,
            Status = RegistrationStatus.ChoDuyet,
            CreatedAt = DateTime.Now
        }));

        return RedirectToAction(nameof(Success), new { id });
    }

    [AllowAnonymous, HttpGet]
    public IActionResult Success(string id)
    {
        var registration = store.Read(d => d.Registrations.FirstOrDefault(r => r.Id == id));
        return registration is null ? RedirectToAction(nameof(Index)) : View(registration);
    }
}
