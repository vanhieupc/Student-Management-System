using BTTH_05.Services;
using System.Text;
using Microsoft.EntityFrameworkCore;
using BTTH_05.Models;
using System.Globalization;
using Microsoft.AspNetCore.Localization;

// Cấu hình encoding cho tiếng Việt để xử lý file/chuỗi không bị lỗi font
Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

var builder = WebApplication.CreateBuilder(args);

// 1. Cấu hình DbContext kết nối SQL Server (Cái này giúp AccountController gọi được DB)
builder.Services.AddDbContext<QuanLySinhVienDContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Đăng ký các Service (Dùng Scoped là chuẩn cho ứng dụng Web)
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<IDiemService, DiemService>();

// --- THÀNH PHẦN THIẾU 1: ĐĂNG KÝ HỆ THỐNG SESSION (Quản lý trạng thái Đăng nhập) ---
builder.Services.AddHttpContextAccessor();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Sau 30 phút không hoạt động sẽ tự đăng xuất
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// --- CẤU HÌNH TIẾNG VIỆT ĐỂ ĐỊNH DẠNG NGÀY THÁNG/TIỀN TỆ CHUẨN VN ---
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[] { new CultureInfo("vi-VN") };
    options.DefaultRequestCulture = new RequestCulture("vi-VN");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});

// 3. Đăng ký Controllers và Views
builder.Services.AddControllersWithViews();
builder.Services.AddLocalization(opt => opt.ResourcesPath = "Resources");

var app = builder.Build();

// --- KÍCH HOẠT CÁC CẤU HÌNH ---
// Phải đặt Localization ở trên cùng để áp dụng cho mọi request
app.UseRequestLocalization();

if (!app.Environment.IsDevelopment())
{

    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

// Nhận diện các file trong wwwroot (như ảnh bg-dainam.jpg)
app.UseStaticFiles();

app.UseRouting();

// --- THÀNH PHẦN THIẾU 2: KÍCH HOẠT SESSION ĐỂ SỬ DỤNG ---
app.UseSession();

app.UseAuthorization();

// Map các file static mới (nếu có dùng .NET 9 assets)
app.MapStaticAssets();

// 4. Cấu hình Route: Đưa trang Đăng nhập Account/Login lên làm mặc định ban đầu
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}")
    .WithStaticAssets();

app.Run();
