using FoodServiceApp.Maui.Services;

namespace FoodServiceApp.Maui.Views;

public partial class AdminVendorApprovalPage : ContentPage
{
    private readonly VendorRegistrationApiClient _apiClient;

    public AdminVendorApprovalPage()
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
            var choDuyet = await _apiClient.GetPendingAsync();

            DsChoDuyet.ItemsSource = choDuyet;
            LblRong.IsVisible = choDuyet.Count == 0;
        }
        catch (Exception ex)
        {
            DsChoDuyet.ItemsSource = null;
            LblRong.IsVisible = true;

            await DisplayAlert(
                "Không tải được dữ liệu",
                $"Không thể kết nối tới Web API.\n\n{ex.Message}",
                "OK");
        }
    }

    private async void OnDuyetClicked(object sender, EventArgs e)
    {
        if (sender is not Button
            {
                CommandParameter: VendorRegistrationDto registration
            })
        {
            return;
        }

        bool xacNhan = await DisplayAlert(
            "Duyệt hồ sơ",
            $"Duyệt gian hàng \"{registration.StoreName}\"?",
            "Duyệt",
            "Để sau");

        if (!xacNhan)
            return;

        try
        {
            bool thanhCong =
                await _apiClient.ApproveAsync(registration.Id);

            if (!thanhCong)
            {
                await DisplayAlert(
                    "Không thành công",
                    "Server không thể duyệt hồ sơ này.",
                    "OK");

                return;
            }

            await NapDanhSachAsync();

            await DisplayAlert(
                "Đã duyệt",
                $"Gian hàng \"{registration.StoreName}\" đã được duyệt.",
                "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert(
                "Lỗi kết nối",
                $"Không thể kết nối tới Web API.\n\n{ex.Message}",
                "OK");
        }
    }

    private async void OnTuChoiClicked(object sender, EventArgs e)
    {
        if (sender is not Button
            {
                CommandParameter: VendorRegistrationDto registration
            })
        {
            return;
        }

        bool xacNhan = await DisplayAlert(
            "Từ chối hồ sơ",
            $"Từ chối hồ sơ đăng ký của \"{registration.StoreName}\"?",
            "Từ chối",
            "Để sau");

        if (!xacNhan)
            return;

        try
        {
            bool thanhCong =
                await _apiClient.RejectAsync(registration.Id);

            if (!thanhCong)
            {
                await DisplayAlert(
                    "Không thành công",
                    "Server không thể từ chối hồ sơ này.",
                    "OK");

                return;
            }

            await NapDanhSachAsync();

            await DisplayAlert(
                "Đã từ chối",
                $"Hồ sơ của \"{registration.StoreName}\" đã bị từ chối.",
                "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert(
                "Lỗi kết nối",
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
            RefreshDsChoDuyet.IsRefreshing = false;
        }
    }
}