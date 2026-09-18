using FoodServiceApp.Maui.Models;

namespace FoodServiceApp.Maui.Views;

public partial class AdminVendorApprovalPage : ContentPage
{
    public AdminVendorApprovalPage()
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
        // TODO: thay bằng GET /api/quan-tri/gian-hang?trangThaiDangKy=ChoDuyet khi có API thật.
        var choDuyet = MockData.DanhSachGianHang
            .Where(g => g.TrangThaiDangKy == TrangThaiDangKyGianHang.ChoDuyet)
            .OrderBy(g => g.NgayDangKy)
            .ToList();

        DsChoDuyet.ItemsSource = choDuyet;
        LblRong.IsVisible = choDuyet.Count == 0;
    }

    private async void OnDuyetClicked(object sender, EventArgs e)
    {
        if (sender is not Button { CommandParameter: GianHang gh }) return;

        gh.TrangThaiDangKy = TrangThaiDangKyGianHang.DaDuyet;
        gh.DangMoCua = true;
        // TODO: gọi PUT /api/quan-tri/gian-hang/{id}/duyet khi có API thật (kèm gửi email thông báo).

        NapDanhSach();
        await DisplayAlert("Đã duyệt", $"Gian hàng \"{gh.TenGianHang}\" đã được kích hoạt trên nền tảng.", "OK");
    }

    private async void OnTuChoiClicked(object sender, EventArgs e)
    {
        if (sender is not Button { CommandParameter: GianHang gh }) return;

        bool xacNhan = await DisplayAlert("Từ chối hồ sơ", $"Từ chối hồ sơ đăng ký của \"{gh.TenGianHang}\"?", "Từ chối", "Để sau");
        if (!xacNhan) return;

        gh.TrangThaiDangKy = TrangThaiDangKyGianHang.TuChoi;
        // TODO: gọi PUT /api/quan-tri/gian-hang/{id}/tu-choi khi có API thật (kèm gửi email lý do).

        NapDanhSach();
    }

    private void OnRefreshing(object? sender, EventArgs e)
    {
        NapDanhSach();
        RefreshDsChoDuyet.IsRefreshing = false;
    }
}
