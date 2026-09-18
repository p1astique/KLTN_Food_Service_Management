using FoodServiceApp.Maui.Models;

namespace FoodServiceApp.Maui.Views;

public partial class AdminVendorListPage : ContentPage
{
    public AdminVendorListPage()
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
        // TODO: thay bằng GET /api/quan-tri/gian-hang khi có API thật.
        var danhSach = MockData.DanhSachGianHang
            .Where(g => g.TrangThaiDangKy == TrangThaiDangKyGianHang.DaDuyet)
            .OrderByDescending(g => g.NgayDangKy)
            .ToList();

        DsGianHang.ItemsSource = danhSach;
    }

    private async void OnKhoaMoClicked(object sender, EventArgs e)
    {
        if (sender is not Button { CommandParameter: GianHang gh }) return;

        if (gh.TaiKhoanBiKhoa)
        {
            gh.TaiKhoanBiKhoa = false;
            // TODO: gọi PUT /api/quan-tri/gian-hang/{id}/mo-khoa khi có API thật.
            NapDanhSach();
            await DisplayAlert("Đã mở khóa", $"Tài khoản gian hàng \"{gh.TenGianHang}\" đã được kích hoạt lại.", "OK");
            return;
        }

        bool xacNhan = await DisplayAlert(
            "Khóa tài khoản gian hàng",
            $"Khóa tài khoản \"{gh.TenGianHang}\"? Gian hàng sẽ không thể đăng nhập hoặc nhận đơn mới cho đến khi được mở lại.",
            "Khóa", "Hủy");
        if (!xacNhan) return;

        gh.TaiKhoanBiKhoa = true;
        // TODO: gọi PUT /api/quan-tri/gian-hang/{id}/khoa khi có API thật (kèm lý do khóa).

        NapDanhSach();
    }

    private void OnRefreshing(object? sender, EventArgs e)
    {
        NapDanhSach();
        RefreshDsGianHang.IsRefreshing = false;
    }
}
