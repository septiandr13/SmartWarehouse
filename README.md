Berikut versi yang sudah dirapikan, profesional, dan siap langsung kamu pakai untuk dokumentasi GitHub (README.md):

---

# 📦 SmartWarehouse System

**SmartWarehouse** adalah aplikasi **Warehouse Management System (WMS)** profesional berbasis **ASP.NET Core MVC** yang dirancang dengan arsitektur scalable, aman, dan mendukung integrasi perangkat seperti **Barcode Scanner**.

---

## 🛠 Tech Stack

* **Backend**: .NET 8 / ASP.NET Core MVC
* **Database**: SQL Server (Entity Framework Core)
* **Authentication**: Microsoft Identity (Role: Admin, Operator)
* **Frontend**: Bootstrap 5, Bootstrap Icons, jQuery
* **Components**:

  * Syncfusion (Theme Switcher)
  * Html5-QRCode (Barcode Scanner)
  * Dropzone.js (File Upload)
* **Reporting**: Rotativa (PDF Generation)

---

## 🚀 Panduan Setup Awal

### 1. Prasyarat (Prerequisites)

Pastikan Anda telah menginstall:

* Visual Studio 2022
* SQL Server (LocalDB atau SQL Express)
* Node.js *(opsional, jika menggunakan dependensi frontend tambahan)*

---

### 2. Konfigurasi Database

Edit file `appsettings.json` dan pastikan connection string sesuai dengan environment Anda:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=SmartWarehouseDb;Trusted_Connection=True;MultipleActiveResultSets=true"
},
"Jwt": {
  "Key": "IniAdalahKunciRahasiaSmartWarehouse2026",
  "Issuer": "SmartWarehouseApp",
  "Audience": "SmartWarehouseUsers"
}
```

---

### 3. Migrasi & Seeding Database

Jalankan perintah berikut di **Package Manager Console**:

```powershell
Add-Migration InitialCreate
Update-Database
```

📌 **Catatan:**

* Saat pertama kali dijalankan, sistem otomatis membuat akun admin:

  * **Email**: `admin@warehouse.com`
  * **Password**: `Admin123!`

---

## 🏗 Struktur Arsitektur & Fitur Utama

### 🔐 Keamanan & Identity

* Menggunakan **ASP.NET Core Identity**
* Mendukung:

  * Role-Based Access Control

    ```csharp
    @if (User.IsInRole("Admin"))
    ```
  * Integrasi halaman Identity (`asp-area="Identity"`)
  * Password Policy
  * Account Lockout
  * Two-Factor Authentication (2FA)

---

### 📦 Inventory & Barcode Scanning

* **Barcode Scanner**:

  * Menggunakan Html5-QRCode
  * Mendukung kamera (HP/Laptop) dan upload gambar

* **Auto-Fill Produk**:

  * AJAX `GetNameBySKU` di `ProductController`
  * Otomatis mengisi nama produk berdasarkan SKU

---

### 🎨 Theme Management

* Integrasi **Syncfusion Theme Switcher**

* Tema tersedia:

  * Material Dark
  * Fluent
  * Bootstrap 5

* Menggunakan **CSS Variables (`:root`)**

* Dikelola via JavaScript:

  ```javascript
  applyTheme()
  ```

---

### 📂 File Management

* Lokasi penyimpanan:

  ```
  /wwwroot/uploads/
  ```
* Nama file menggunakan **GUID** untuk menghindari konflik
* Upload file menggunakan **Dropzone.js** (drag & drop)

---

## 📝 Troubleshooting

| Masalah                                   | Solusi                                                                                               |
| ----------------------------------------- | ---------------------------------------------------------------------------------------------------- |
| Email reset password tidak terkirim       | Pastikan `AddDefaultTokenProviders()` ada di `Program.cs` dan App Password Gmail sudah dikonfigurasi |
| Sidebar tidak responsif di mobile         | Periksa `_Layout.cshtml`, pastikan `sidebar-overlay` dan event resize aktif                          |
| Halaman Identity tidak menggunakan layout | Pastikan `_ViewStart.cshtml` di `Areas/Identity/Pages/` mengarah ke layout utama                     |
| Swagger tidak muncul                      | Pastikan konfigurasi Swagger dilakukan sebelum `builder.Build()` di `Program.cs`                     |

---

## 📌 Catatan Tambahan

* Pastikan environment development sudah sesuai sebelum menjalankan aplikasi
* Gunakan HTTPS untuk keamanan maksimal saat deployment
* Disarankan menggunakan SQL Server versi terbaru untuk performa optimal

---
