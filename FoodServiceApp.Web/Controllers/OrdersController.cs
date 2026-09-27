using FoodServiceApp.Web.Models;
using FoodServiceApp.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodServiceApp.Web.Controllers;

[Authorize]
public sealed class OrdersController(VendorDataStore store) : Controller
{
    public IActionResult Index(OrderStatus? status = null)
    {
        ViewBag.Status = status;
        ViewBag.Statuses = Enum.GetValues<OrderStatus>();
        var orders = store.Read(d => d.Orders.OrderByDescending(o => o.CreatedAt)
            .Where(o => status is null || o.Status == status).ToList());
        return View(orders);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult UpdateStatus(string id, OrderStatus status)
    {
        store.Update(d => { var order = d.Orders.FirstOrDefault(o => o.Id == id); if (order is not null) order.Status = status; });
        TempData["Success"] = "Đã cập nhật trạng thái đơn hàng.";
        return RedirectToAction(nameof(Index));
    }
}
