# Bếp Nhà — cổng quản lý gian hàng

Website ASP.NET Core MVC chạy độc lập bên cạnh app .NET MAUI.

## Chạy trong VS Code

Mở thư mục project gốc, nhấn `Ctrl+Shift+P` → **Tasks: Run Task** → **WEB: Run vendor portal**. Website mở tại `http://localhost:5234`.

Hoặc chạy trong terminal ở thư mục gốc:

```powershell
dotnet user-secrets set "DemoAccount:Password" "<mat-khau-demo-cua-ban>" --project FoodServiceApp.Web
dotnet run --project FoodServiceApp.Web/FoodServiceApp.Web.csproj --launch-profile http
```

Email demo: `comtam.saigon@gianhang.vn`. Lệnh đầu tiên lưu mật khẩu demo riêng trên máy, không đưa mật khẩu vào Git.

## Phạm vi hiện tại

- Đăng nhập và đăng xuất gian hàng bằng cookie.
- Trang tổng quan, hồ sơ gian hàng, danh mục, món ăn và đơn hàng.
- CRUD danh mục/món; có lọc danh mục, trạng thái còn hàng và cập nhật trạng thái đơn.
- Dữ liệu khởi tạo mẫu được lưu tại `App_Data/vendor-data.json` và giữ sau khi khởi động lại.

Dữ liệu JSON phù hợp để chạy demo trên một máy. Website chưa kết nối SQL Server/API của app MAUI, chưa có nhiều tài khoản/phân quyền thật; trước khi triển khai sử dụng thực tế cần thay bằng backend và database phù hợp, đồng thời đổi cấu hình tài khoản demo.
