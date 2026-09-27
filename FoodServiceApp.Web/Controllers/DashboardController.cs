using FoodServiceApp.Web.Models;
using FoodServiceApp.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodServiceApp.Web.Controllers;

[Authorize]
public sealed class DashboardController(VendorDataStore store) : Controller
{
    public IActionResult Index()
    {
        var model = store.Read(d => new DashboardViewModel
        {
            Store = d.Store,
            RecentOrders = d.Orders.OrderByDescending(o => o.CreatedAt).Take(5).ToList(),
            PendingCount = d.Orders.Count(o => o.Status == OrderStatus.ChoXacNhan),
            FoodCount = d.Foods.Count,
            TodayRevenue = d.Orders.Where(o => o.CreatedAt.Date == DateTime.Today && o.Status == OrderStatus.HoanThanh).Sum(o => o.Total),
            TodayCompleted = d.Orders.Count(o => o.CreatedAt.Date == DateTime.Today && o.Status == OrderStatus.HoanThanh)
        });
        return View(model);
    }
}
