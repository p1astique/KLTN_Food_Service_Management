using FoodServiceApp.Maui.Services;

namespace FoodServiceApp.Maui.Views;

public partial class AdminVendorListPage : ContentPage
{
    private readonly VendorRegistrationApiClient _apiClient;

    public AdminVendorListPage()
    {
        InitializeComponent();
        _apiClient = new VendorRegistrationApiClient();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await NapDanhSachAsync();
    }

    private async Task NapDanhSachAsync()
    {
        try
        {
            var tatCa = await _apiClient.GetAllAsync();

            var daDuyet = tatCa
                .Where(r => r.Status == 1)
                .OrderByDescending(r => r.CreatedAt)
                .ToList();

            DsGianHang.ItemsSource = daDuyet;
        }
        catch (Exception ex)
        {
            DsGianHang.ItemsSource = null;

            await DisplayAlert(
                "Không tải được dữ liệu",
                $"Không thể kết nối tới Web API.\n\n{ex.Message}",
                "OK");
        }
    }

    private async void OnRefreshing(object? sender, EventArgs e)
    {
        try
        {
            await NapDanhSachAsync();
        }
        finally
        {
            RefreshDsGianHang.IsRefreshing = false;
        }
    }
}