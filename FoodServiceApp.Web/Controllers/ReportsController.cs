using FoodServiceApp.Web.Models;
using FoodServiceApp.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodServiceApp.Web.Controllers;

[Authorize]
public sealed class ReportsController(VendorDataStore store) : Controller
{
    public IActionResult Index()
    {
        var today = DateTime.Today;
        var monthStart = new DateTime(today.Year, today.Month, 1);
        var model = store.Read(d =>
        {
            var completed = d.Orders.Where(o => o.Status == OrderStatus.HoanThanh).ToList();
            var monthCompleted = completed.Where(o => o.CreatedAt >= monthStart).ToList();
            return new ReportViewModel
            {
                RevenueToday = completed.Where(o => o.CreatedAt.Date == today).Sum(o => o.Total),
                RevenueThisMonth = monthCompleted.Sum(o => o.Total),
                CompletedOrders = monthCompleted.Count,
                CancelledOrders = d.Orders.Count(o => o.CreatedAt >= monthStart && o.Status is OrderStatus.DaHuy or OrderStatus.DaTuChoi),
                AverageOrderValue = monthCompleted.Count == 0 ? 0 : monthCompleted.Average(o => o.Total),
                DailyRevenue = Enumerable.Range(0, 7).Select(offset =>
                {
                    var date = today.AddDays(-6 + offset);
                    var orders = completed.Where(o => o.CreatedAt.Date == date).ToList();
                    return new DailyRevenue { Date = date, Revenue = orders.Sum(o => o.Total), Orders = orders.Count };
                }).ToList()
            };
        });
        return View(model);
    }
}
