using FoodServiceApp.Web.Models;
using FoodServiceApp.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodServiceApp.Web.Controllers;

[Authorize]
public sealed class StoreController(VendorDataStore store) : Controller
{
    [HttpGet]
    public IActionResult Profile() => View(store.Read(d => d.Store));

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Profile(StoreProfile input)
    {
        if (string.IsNullOrWhiteSpace(input.Name) || string.IsNullOrWhiteSpace(input.Address) ||
            string.IsNullOrWhiteSpace(input.Phone) || input.DeliveryMinutes is < 1 or > 300)
        {
            ModelState.AddModelError(string.Empty, "Vui lòng nhập đủ thông tin hợp lệ; thời gian giao từ 1 đến 300 phút.");
            return View(input);
        }

        store.Update(d =>
        {
            input.Id = d.Store.Id;
            d.Store = input;
        });
        TempData["Success"] = "Đã cập nhật thông tin gian hàng.";
        return RedirectToAction(nameof(Profile));
    }
}
