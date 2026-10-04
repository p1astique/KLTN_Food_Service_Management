# Food Service Management — .NET MAUI + ASP.NET Core

Đồ án quản lý dịch vụ ăn uống gồm ứng dụng **.NET MAUI** và cổng quản lý **ASP.NET Core Web**. Repository hiện đang được phát triển theo từng luồng chức năng; branch `feature/vendor-weekly-functions` tập trung vào **Đối tác/Gian hàng** và **Khuyến mãi**.

## Công nghệ

- .NET 8 / .NET MAUI
- ASP.NET Core MVC / Web API
- XAML
- C#
- Dữ liệu demo cục bộ bằng JSON và `MockData` ở các phần chưa nối backend thật

## Các luồng chính

### Khách hàng

Ứng dụng MAUI đã có các màn hình đăng nhập/đăng ký, trang chủ, thực đơn, giỏ hàng, thanh toán, theo dõi đơn hàng, đánh giá, hoạt động, ví tiền, tin nhắn và tài khoản.

### Đối tác / Gian hàng

Phần gian hàng hiện có:

- Đăng ký gian hàng.
- Đăng nhập và dashboard gian hàng.
- Quản lý thông tin cửa hàng và trạng thái kinh doanh.
- Quản lý danh mục và món ăn.
- Thêm / sửa / xóa món.
- Cập nhật giá, thông tin, hình ảnh và trạng thái còn/hết món.
- Tiếp nhận và xử lý đơn hàng theo các bước xác nhận/từ chối → chuẩn bị → sẵn sàng giao → bàn giao tài xế.
- Theo dõi doanh thu và báo cáo kinh doanh.
- Quản lý chương trình khuyến mãi và voucher.

Cổng Web hỗ trợ upload ảnh món ăn vào `wwwroot/uploads/foods`.

### Duyệt đăng ký gian hàng

Luồng đăng ký và duyệt gian hàng đã được nối giữa ASP.NET Core Web và ứng dụng MAUI Admin:

```text
Web đăng ký gian hàng
        ↓
vendor-data.json
        ↓
GET /api/vendor-registrations/pending
        ↓
MAUI Admin xem hồ sơ chờ duyệt
        ↓
Duyệt / Từ chối
        ↓
Web cập nhật trạng thái hồ sơ
        ↓
Danh sách gian hàng hiển thị các hồ sơ đã duyệt
```

Các endpoint hiện có:

```text
GET  /api/vendor-registrations
GET  /api/vendor-registrations/pending
POST /api/vendor-registrations/{id}/approve
POST /api/vendor-registrations/{id}/reject
```

`AdminVendorApprovalPage` và `AdminVendorListPage` hiện lấy dữ liệu đăng ký từ Web API thay vì chỉ sử dụng `MockData`.

> Lưu ý: danh sách gian hàng hiện đang dựa trên **hồ sơ đăng ký đã duyệt**. Việc tạo entity/tài khoản gian hàng hoàn chỉnh sau khi duyệt, khóa/mở khóa tài khoản và liên kết trạng thái kinh doanh với backend vẫn là phần cần hoàn thiện.

### Khuyến mãi

Hệ thống đã có giao diện và logic demo cho:

- Tạo mã giảm giá/voucher.
- Giảm theo phần trăm hoặc số tiền cố định.
- Thiết lập thời gian áp dụng.
- Điều kiện đơn hàng tối thiểu và giới hạn giảm.
- Phạm vi áp dụng theo hệ thống/gian hàng.
- Sửa, bật/tắt hoặc ngừng chương trình.
- Hiển thị số lượt sử dụng để phục vụ theo dõi hiệu quả.

Phần áp dụng voucher vào checkout thật và cập nhật lượt sử dụng tự động vẫn cần nối với backend/dữ liệu đơn hàng thực tế.

### Quản trị

Ứng dụng MAUI có các màn hình quản trị cho dashboard, duyệt hồ sơ gian hàng, danh sách gian hàng, quản lý khách hàng, khiếu nại/vấn đề, khuyến mãi và thống kê.

Một số màn hình quản trị vẫn sử dụng `MockData`; riêng luồng duyệt đăng ký gian hàng đã gọi ASP.NET Core Web API.

## Chạy project

### ASP.NET Core Web

Từ thư mục gốc:

```powershell
dotnet build .\FoodServiceApp.Web\FoodServiceApp.Web.csproj
dotnet run --project .\FoodServiceApp.Web\FoodServiceApp.Web.csproj
```

Web mặc định trong môi trường phát triển hiện tại:

```text
http://localhost:5234
```

Có thể kiểm tra API đăng ký bằng:

```text
http://localhost:5234/api/vendor-registrations
http://localhost:5234/api/vendor-registrations/pending
```

### .NET MAUI Windows

```powershell
dotnet build .\FoodServiceApp.Maui.csproj -f net8.0-windows10.0.19041.0
dotnet run --project .\FoodServiceApp.Maui.csproj -f net8.0-windows10.0.19041.0
```

Khi test luồng Admin ↔ Web, cần giữ Web chạy song song với ứng dụng MAUI.

## Cấu trúc liên quan

```text
FoodServiceApp.Maui/
├── Models/
├── Services/
│   └── VendorRegistrationApiClient.cs
├── Views/
│   ├── AdminVendorApprovalPage.xaml
│   ├── AdminVendorListPage.xaml
│   └── ...
└── FoodServiceApp.Web/
    ├── Controllers/
    │   ├── CatalogController.cs
    │   └── VendorRegistrationsApiController.cs
    ├── Models/
    ├── Services/
    ├── Views/
    └── wwwroot/
```

## Trạng thái hiện tại

Project đang ở giai đoạn tích hợp dần dữ liệu thật giữa MAUI và ASP.NET Core. Không nên hiểu toàn bộ ứng dụng đã sử dụng database/API thật.

Các phần cần tiếp tục hoàn thiện gồm:

- Tạo gian hàng/tài khoản gian hàng hoàn chỉnh sau khi Admin duyệt hồ sơ.
- Khóa/mở khóa tài khoản gian hàng qua backend.
- Đồng bộ trạng thái mở/đóng cửa với backend.
- Kết nối các màn hình MAUI còn dùng `MockData` sang API thật.
- Hoàn thiện dữ liệu doanh thu, thống kê và hiệu quả khuyến mãi từ đơn hàng thực tế.
- Hoàn thiện xác thực và phân quyền cho môi trường production.

## Ghi chú phát triển

Thư mục `FoodServiceApp.Web` là project riêng nằm bên trong repository MAUI và đã được loại khỏi quá trình compile của `FoodServiceApp.Maui.csproj`.

Không commit các file sinh ra khi build trong `bin/` và `obj/`. Khi commit thay đổi, nên stage từng file source cần thiết thay vì dùng `git add .`.
