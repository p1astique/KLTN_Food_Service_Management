using FoodServiceApp.Web.Models;
using FoodServiceApp.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodServiceApp.Web.Controllers;

[Authorize]
public sealed class CatalogController(VendorDataStore store) : Controller
{
    public IActionResult Index(string? category = null, string? edit = null)
    {
        var model = store.Read(d => new CatalogViewModel
        {
            Categories = d.Categories.ToList(),
            Foods = d.Foods.Where(f => string.IsNullOrEmpty(category) || f.CategoryId == category).ToList(),
            CategoryNames = d.Categories.ToDictionary(c => c.Id, c => c.Name),
            SelectedCategory = category,
            Form = d.Foods.FirstOrDefault(f => f.Id == edit) is { } food
                ? new FoodFormModel { Id = food.Id, Name = food.Name, CategoryId = food.CategoryId, Price = food.Price, Emoji = food.Emoji, Description = food.Description, InStock = food.InStock }
                : new FoodFormModel()
        });
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult AddCategory(string name)
    {
        name = name?.Trim() ?? "";
        if (name.Length is < 2 or > 50) TempData["Error"] = "Tên danh mục cần từ 2 đến 50 ký tự.";
        else
        {
            var exists = store.Read(d => d.Categories.Any(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase)));
            if (exists) TempData["Error"] = "Danh mục này đã tồn tại.";
            else
            {
                store.Update(d => d.Categories.Add(new FoodCategory { Name = name }));
                TempData["Success"] = "Đã thêm danh mục.";
            }
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult RenameCategory(string id, string name)
    {
        name = name?.Trim() ?? "";
        var valid = name.Length is >= 2 and <= 50 && store.Read(d => !d.Categories.Any(c => c.Id != id && c.Name.Equals(name, StringComparison.OrdinalIgnoreCase)));
        if (!valid) TempData["Error"] = "Tên danh mục không hợp lệ hoặc đã được sử dụng.";
        else
        {
            store.Update(d => { var category = d.Categories.FirstOrDefault(c => c.Id == id); if (category is not null) category.Name = name; });
            TempData["Success"] = "Đã đổi tên danh mục.";
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteCategory(string id, string? moveToCategoryId)
    {
        var outcome = store.Read(d =>
        {
            if (!d.Categories.Any(c => c.Id == id)) return "missing";
            var hasFoods = d.Foods.Any(f => f.CategoryId == id);
            if (hasFoods && (string.IsNullOrEmpty(moveToCategoryId) || moveToCategoryId == id || !d.Categories.Any(c => c.Id == moveToCategoryId))) return "needs-target";
            return "ok";
        });
        if (outcome == "needs-target") TempData["Error"] = "Danh mục còn món. Hãy chọn danh mục nhận món trước khi xóa.";
        else if (outcome == "ok")
        {
            store.Update(d =>
            {
                foreach (var food in d.Foods.Where(f => f.CategoryId == id)) food.CategoryId = moveToCategoryId!;
                d.Categories.RemoveAll(c => c.Id == id);
            });
            TempData["Success"] = "Đã xóa danh mục.";
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult SaveFood(FoodFormModel input)
    {
        var hasCategory = store.Read(d => d.Categories.Any(c => c.Id == input.CategoryId));
        if (string.IsNullOrWhiteSpace(input.Name) || input.Name.Trim().Length > 100 || input.Price <= 0 || !hasCategory)
        {
            TempData["Error"] = "Tên món, giá bán và danh mục cần hợp lệ.";
            return RedirectToAction(nameof(Index));
        }
        store.Update(d =>
        {
            var item = string.IsNullOrWhiteSpace(input.Id) ? null : d.Foods.FirstOrDefault(f => f.Id == input.Id);
            if (item is null) { item = new FoodItem { Id = Guid.NewGuid().ToString("N") }; d.Foods.Add(item); }
            item.Name = input.Name.Trim(); item.CategoryId = input.CategoryId; item.Price = input.Price;
            item.Emoji = string.IsNullOrWhiteSpace(input.Emoji) ? "🍲" : input.Emoji.Trim();
            item.Description = input.Description?.Trim() ?? ""; item.InStock = input.InStock;
        });
        TempData["Success"] = "Đã lưu món ăn.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ToggleStock(string id)
    {
        store.Update(d => { var item = d.Foods.FirstOrDefault(f => f.Id == id); if (item is not null) item.InStock = !item.InStock; });
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteFood(string id)
    {
        store.Update(d => d.Foods.RemoveAll(f => f.Id == id));
        TempData["Success"] = "Đã xóa món ăn.";
        return RedirectToAction(nameof(Index));
    }
}
