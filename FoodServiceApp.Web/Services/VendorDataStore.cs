using System.Text.Json;
using FoodServiceApp.Web.Models;

namespace FoodServiceApp.Web.Services;

/// <summary>Local JSON storage for the standalone demo website.</summary>
public sealed class VendorDataStore
{
    private readonly object _gate = new();
    private readonly string _path;
    private PortalData _data;
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    public VendorDataStore(IWebHostEnvironment environment)
    {
        var folder = Path.Combine(environment.ContentRootPath, "App_Data");
        Directory.CreateDirectory(folder);
        _path = Path.Combine(folder, "vendor-data.json");
        _data = LoadOrSeed();
    }

    public T Read<T>(Func<PortalData, T> read)
    {
        lock (_gate) return read(_data);
    }

    public void Update(Action<PortalData> update)
    {
        lock (_gate)
        {
            update(_data);
            var temp = _path + ".tmp";
            File.WriteAllText(temp, JsonSerializer.Serialize(_data, JsonOptions));
            File.Move(temp, _path, true);
        }
    }

    private PortalData LoadOrSeed()
    {
        if (File.Exists(_path))
        {
            try
            {
                var loaded = JsonSerializer.Deserialize<PortalData>(File.ReadAllText(_path));
                if (loaded is not null) return loaded;
            }
            catch (JsonException) { }
        }

        var data = new PortalData();
        var main = new FoodCategory { Id = "cat-main", Name = "Món chính" };
        var side = new FoodCategory { Id = "cat-side", Name = "Món phụ" };
        var drink = new FoodCategory { Id = "cat-drink", Name = "Nước uống" };
        var dessert = new FoodCategory { Id = "cat-dessert", Name = "Tráng miệng" };
        data.Categories.AddRange([main, side, drink, dessert]);
        data.Foods.AddRange([
            new FoodItem { Id = "food-1", Name = "Cơm tấm sườn bì chả", CategoryId = main.Id, Price = 45000, Emoji = "🍛", Description = "Sườn nướng, bì, chả trứng." },
            new FoodItem { Id = "food-2", Name = "Cơm tấm sườn nướng", CategoryId = main.Id, Price = 40000, Emoji = "🍖" },
            new FoodItem { Id = "food-3", Name = "Canh khổ qua", CategoryId = side.Id, Price = 15000, Emoji = "🥣" },
            new FoodItem { Id = "food-4", Name = "Trứng ốp la", CategoryId = side.Id, Price = 10000, Emoji = "🍳" },
            new FoodItem { Id = "food-5", Name = "Trà tắc", CategoryId = drink.Id, Price = 12000, Emoji = "🥤" }
        ]);
        data.Orders.AddRange([
            new StoreOrder { Id = "DH3001", Customer = "Nguyễn Minh Anh", Address = "77 Nguyễn Đình Chiểu, Quận 3", CreatedAt = DateTime.Now.AddMinutes(-12), Total = 102000, Status = OrderStatus.ChoXacNhan, Items = "2 × Cơm tấm sườn bì chả" },
            new StoreOrder { Id = "DH3002", Customer = "Trần Quốc Bảo", Address = "205 Võ Văn Tần, Quận 3", CreatedAt = DateTime.Now.AddMinutes(-28), Total = 65000, Status = OrderStatus.DangChuanBi, Items = "1 × Cơm tấm sườn nướng, 1 × Trứng ốp la" },
            new StoreOrder { Id = "DH2999", Customer = "Lê Thu Hà", Address = "9 Cống Quỳnh, Quận 1", CreatedAt = DateTime.Now.AddDays(-1), Total = 57000, Status = OrderStatus.HoanThanh, Items = "1 × Cơm tấm sườn bì chả, 1 × Trà tắc" }
        ]);
        var temp = _path + ".tmp";
        File.WriteAllText(temp, JsonSerializer.Serialize(data, JsonOptions));
        File.Move(temp, _path, true);
        return data;
    }
}
