using FoodServiceApp.Web.Models;
using FoodServiceApp.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodServiceApp.Web.Controllers;

[Authorize]
public sealed class PromotionsController(VendorDataStore store) : Controller
{
    public IActionResult Index(string? edit = null)
    {
        return View(store.Read(d => new PromotionsViewModel
        {
            Promotions = d.Promotions.OrderByDescending(p => p.StartAt).ToList(),
            Foods = d.Foods.ToList(),
            Form = d.Promotions.FirstOrDefault(p => p.Id == edit) is { } p
                ? new PromotionFormModel { Id = p.Id, Name = p.Name, Code = p.Code, Type = p.Type, Value = p.Value, MinimumOrder = p.MinimumOrder, MaximumDiscount = p.MaximumDiscount, StartAt = p.StartAt, EndAt = p.EndAt, FoodId = p.FoodId }
                : new PromotionFormModel()
        }));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Save(PromotionFormModel input)
    {
        input.Name = input.Name?.Trim() ?? ""; input.Code = input.Code?.Trim().ToUpperInvariant() ?? "";
        if (input.Name.Length < 2 || input.Code.Length < 3 || input.Value <= 0 || input.EndAt < input.StartAt ||
            (input.Type == PromotionType.PhanTram && input.Value > 100))
        {
            TempData["Error"] = "Thông tin khuyến mãi chưa hợp lệ.";
            return RedirectToAction(nameof(Index));
        }
        if (store.Read(d => d.Promotions.Any(p => p.Id != input.Id && p.Code.Equals(input.Code, StringComparison.OrdinalIgnoreCase))))
        {
            TempData["Error"] = "Mã giảm giá đã tồn tại.";
            return RedirectToAction(nameof(Index));
        }
        store.Update(d =>
        {
            var p = string.IsNullOrWhiteSpace(input.Id) ? null : d.Promotions.FirstOrDefault(x => x.Id == input.Id);
            if (p is null) { p = new Promotion(); d.Promotions.Add(p); }
            p.Name = input.Name; p.Code = input.Code; p.Type = input.Type; p.Value = input.Value;
            p.MinimumOrder = Math.Max(0, input.MinimumOrder); p.MaximumDiscount = input.MaximumDiscount;
            p.StartAt = input.StartAt; p.EndAt = input.EndAt; p.FoodId = string.IsNullOrWhiteSpace(input.FoodId) ? null : input.FoodId;
        });
        TempData["Success"] = "Đã lưu chương trình khuyến mãi.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Toggle(string id)
    {
        store.Update(d => { var p = d.Promotions.FirstOrDefault(x => x.Id == id); if (p is not null) p.IsActive = !p.IsActive; });
        TempData["Success"] = "Đã cập nhật trạng thái khuyến mãi.";
        return RedirectToAction(nameof(Index));
    }
}
