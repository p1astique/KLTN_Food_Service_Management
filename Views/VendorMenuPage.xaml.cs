using FoodServiceApp.Maui.Models;

namespace FoodServiceApp.Maui.Views;

public partial class VendorMenuPage : ContentPage
{
    public VendorMenuPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        NapDanhSach();
    }

    private void NapDanhSach()
    {
        // TODO: thay bằng GET /api/gian-hang/mon-an khi có API thật.
        var danhSach = MockData.MonAnCuaGianHang(MockData.GianHangHienTai.Id);
        DsMonAn.ItemsSource = danhSach;
        LblRong.IsVisible = danhSach.Count == 0;
    }

    private void OnConHangToggled(object? sender, ToggledEventArgs e)
    {
        if (sender is not Switch { BindingContext: MonAn mon }) return;

        mon.ConHang = e.Value;
        // TODO: gọi PUT /api/gian-hang/mon-an/{id}/tinh-trang { conHang } khi có API thật.
    }

    private async void OnThemMonClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(VendorMenuItemEditPage));
    }

    private async void OnSuaClicked(object sender, EventArgs e)
    {
        if (sender is not Button { CommandParameter: MonAn mon }) return;

        await Shell.Current.GoToAsync(nameof(VendorMenuItemEditPage), new Dictionary<string, object>
        {
            { "MonAnId", mon.Id }
        });
    }

    private async void OnXoaClicked(object sender, EventArgs e)
    {
        if (sender is not Button { CommandParameter: MonAn mon }) return;

        bool xacNhan = await DisplayAlert("Xóa món ăn", $"Xóa \"{mon.TenMon}\" khỏi thực đơn?", "Xóa", "Hủy");
        if (!xacNhan) return;

        MockData.DanhSachMonAn.Remove(mon);
        // TODO: gọi DELETE /api/gian-hang/mon-an/{id} khi có API thật.

        NapDanhSach();
    }

    private void OnRefreshing(object? sender, EventArgs e)
    {
        NapDanhSach();
        RefreshThucDon.IsRefreshing = false;
    }
}
