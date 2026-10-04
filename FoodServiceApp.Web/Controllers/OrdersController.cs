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
        return View(store.Read(d => d.Orders.OrderByDescending(o => o.CreatedAt).Where(o => status is null || o.Status == status).ToList()));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult UpdateStatus(string id, OrderStatus status)
    {
        var result = "missing";
        store.Update(d =>
        {
            var order = d.Orders.FirstOrDefault(o => o.Id == id);
            if (order is null) return;
            if (!CanTransition(order.Status, status)) { result = "invalid"; return; }
            order.Status = status; result = "ok";
        });
        TempData[result == "ok" ? "Success" : "Error"] = result == "ok" ? "Đã cập nhật trạng thái đơn hàng." : "Không thể chuyển đơn sang trạng thái này.";
        return RedirectToAction(nameof(Index));
    }

    private static bool CanTransition(OrderStatus current, OrderStatus next) => current switch
    {
        OrderStatus.ChoXacNhan => next is OrderStatus.DaXacNhan or OrderStatus.DaTuChoi,
        OrderStatus.DaXacNhan => next == OrderStatus.DangChuanBi,
        OrderStatus.DangChuanBi => next == OrderStatus.SanSangGiao,
        OrderStatus.SanSangGiao => next == OrderStatus.DaBanGiaoTaiXe,
        OrderStatus.DaBanGiaoTaiXe => next == OrderStatus.DangGiao,
        OrderStatus.DangGiao => next == OrderStatus.HoanThanh,
        _ => false
    };
}
