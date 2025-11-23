# 🛒 Web Shopping - ASP.NET Core

Đây là dự án website bán hàng trực tuyến được xây dựng bằng **ASP.NET Core MVC**.

## 🚀 Giới thiệu
Website cung cấp các chức năng cơ bản của một trang thương mại điện tử, cho phép người dùng xem sản phẩm, thêm vào giỏ hàng và đặt hàng. Ngoài ra còn có trang Admin để quản lý dữ liệu.

## 🛠️ Công nghệ sử dụng
* **Framework:** ASP.NET Core MVC (.NET 8)
* **Ngôn ngữ:** C#
* **Cơ sở dữ liệu:** SQL Server
* **ORM:** Entity Framework Core (Code First)
* **Frontend:** HTML5, CSS3, Bootstrap, JavaScript/jQuery
* **IDE:** Visual Studio 2022 / VS Code

## ✨ Chức năng chính

### 👤 Khách hàng (User)
* Xem danh sách sản phẩm, chi tiết sản phẩm.
* Tìm kiếm sản phẩm.
* Thêm sản phẩm vào giỏ hàng.
* Quản lý giỏ hàng (thêm, sửa, xóa).
* Thanh toán (Checkout).
* Đăng ký / Đăng nhập tài khoản.

### 🛡️ Quản trị viên (Admin)
* Quản lý Danh mục (Categories).
* Quản lý Sản phẩm (Products).
* Quản lý Đơn hàng (Orders).
* Thống kê doanh thu.

## ⚙️ Hướng dẫn Cài đặt & Chạy (Setup)

Để chạy dự án này trên máy local, bạn làm theo các bước sau:

1.  **Clone dự án:**
    ```bash
    git clone [https://github.com/trungnguyen020/ASP.NET-Web-Shopping.git](https://github.com/trungnguyen020/ASP.NET-Web-Shopping.git)
    ```

2.  **Cấu hình Cơ sở dữ liệu:**
    * Mở file `appsettings.json` (hoặc tạo `appsettings.Development.json`).
    * Chỉnh sửa chuỗi kết nối `ConnectionStrings` phù hợp với SQL Server của bạn.

3.  **Cập nhật Database:**
    Mở **Package Manager Console** trong Visual Studio và chạy lệnh:
    ```powershell
    Update-Database
    ```
    *(Hoặc dùng terminal: `dotnet ef database update`)*

4.  **Chạy ứng dụng:**
    Nhấn `F5` hoặc chạy lệnh `dotnet run`.
---
