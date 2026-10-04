namespace FoodServiceApp.Web.Models;

public sealed class PortalData
{
    public StoreProfile Store { get; set; } = new();
    public List<FoodCategory> Categories { get; set; } = new();
    public List<FoodItem> Foods { get; set; } = new();
    public List<StoreOrder> Orders { get; set; } = new();
    public List<Promotion> Promotions { get; set; } = new();
}

public sealed class StoreProfile
{
    public string Id { get; set; } = "gh1";
    public string Name { get; set; } = "Cơm Tấm Sài Gòn";
    public string Address { get; set; } = "12 Nguyễn Trãi, Quận 5, TP. Hồ Chí Minh";
    public string Phone { get; set; } = "0908 111 222";
    public string Email { get; set; } = "comtam.saigon@gianhang.vn";
    public string Description { get; set; } = "Cơm tấm sườn bì chả chuẩn vị Sài Gòn.";
    public int DeliveryMinutes { get; set; } = 25;
    public bool IsOpen { get; set; } = true;
}

public sealed class FoodCategory
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public string Name { get; set; } = "";
}

public sealed class FoodItem
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public string Name { get; set; } = "";
    public string CategoryId { get; set; } = "";
    public decimal Price { get; set; }
    public string Emoji { get; set; } = "🍲";
    public string ImageUrl { get; set; } = "";
    public bool InStock { get; set; } = true;
    public string Description { get; set; } = "";
}

public enum OrderStatus
{
    // Giữ nguyên các giá trị cũ vì vendor-data.json lưu enum dưới dạng số.
    ChoXacNhan = 0,
    DangChuanBi = 1,
    DangGiao = 2,
    HoanThanh = 3,
    DaHuy = 4,

    // Trạng thái bổ sung cho workflow gian hàng.
    DaXacNhan = 5,
    SanSangGiao = 6,
    DaBanGiaoTaiXe = 7,
    DaTuChoi = 8
}

public sealed class StoreOrder
{
    public string Id { get; set; } = "";
    public string Customer { get; set; } = "";
    public string Address { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public decimal Total { get; set; }
    public OrderStatus Status { get; set; }
    public string Items { get; set; } = "";
}

public enum PromotionType { PhanTram, SoTien }

public sealed class Promotion
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public string Name { get; set; } = "";
    public string Code { get; set; } = "";
    public PromotionType Type { get; set; } = PromotionType.PhanTram;
    public decimal Value { get; set; }
    public decimal MinimumOrder { get; set; }
    public decimal? MaximumDiscount { get; set; }
    public DateTime StartAt { get; set; } = DateTime.Today;
    public DateTime EndAt { get; set; } = DateTime.Today.AddDays(7);
    public bool IsActive { get; set; } = true;
    public string? FoodId { get; set; }
    public int UsageCount { get; set; }
}

public sealed class PromotionFormModel
{
    public string? Id { get; set; }
    public string Name { get; set; } = "";
    public string Code { get; set; } = "";
    public PromotionType Type { get; set; }
    public decimal Value { get; set; }
    public decimal MinimumOrder { get; set; }
    public decimal? MaximumDiscount { get; set; }
    public DateTime StartAt { get; set; } = DateTime.Today;
    public DateTime EndAt { get; set; } = DateTime.Today.AddDays(7);
    public string? FoodId { get; set; }
}

public sealed class PromotionsViewModel
{
    public required IReadOnlyList<Promotion> Promotions { get; init; }
    public required IReadOnlyList<FoodItem> Foods { get; init; }
    public PromotionFormModel Form { get; init; } = new();
}

public sealed class ReportViewModel
{
    public decimal RevenueToday { get; init; }
    public decimal RevenueThisMonth { get; init; }
    public int CompletedOrders { get; init; }
    public int CancelledOrders { get; init; }
    public decimal AverageOrderValue { get; init; }
    public required IReadOnlyList<DailyRevenue> DailyRevenue { get; init; }
}

public sealed class DailyRevenue
{
    public DateTime Date { get; init; }
    public decimal Revenue { get; init; }
    public int Orders { get; init; }
}

public sealed class DashboardViewModel
{
    public required StoreProfile Store { get; init; }
    public required IReadOnlyList<StoreOrder> RecentOrders { get; init; }
    public int PendingCount { get; init; }
    public int FoodCount { get; init; }
    public decimal TodayRevenue { get; init; }
    public int TodayCompleted { get; init; }
}

public sealed class FoodFormModel
{
    public string? Id { get; set; }
    public string Name { get; set; } = "";
    public string CategoryId { get; set; } = "";
    public decimal Price { get; set; }
    public string Emoji { get; set; } = "🍲";
    public string ImageUrl { get; set; } = "";
    public string Description { get; set; } = "";
    public bool InStock { get; set; } = true;
}

public sealed class CatalogViewModel
{
    public required IReadOnlyList<FoodCategory> Categories { get; init; }
    public required IReadOnlyList<FoodItem> Foods { get; init; }
    public required IReadOnlyDictionary<string, string> CategoryNames { get; init; }
    public string? SelectedCategory { get; init; }
    public FoodFormModel Form { get; init; } = new();
}
