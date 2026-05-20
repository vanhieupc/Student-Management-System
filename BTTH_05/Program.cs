using BTTH_05.Services;
using System.Text;
using Microsoft.EntityFrameworkCore;
using BTTH_05.Models;
using System.Globalization;
using Microsoft.AspNetCore.Localization;

// Cấu hình encoding cho tiếng Việt
Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

var builder = WebApplication.CreateBuilder(args);

// 1. Cấu hình DbContext kết nối SQL Server
builder.Services.AddDbContext<QuanLySinhVienDContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Đăng ký các Service
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<IDiemService, DiemService>();

// --- CẤU HÌNH TIẾNG VIỆT ĐỂ KHÔNG LỖI NGÀY THÁNG ---
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[] { new CultureInfo("vi-VN") };
    options.DefaultRequestCulture = new RequestCulture("vi-VN");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});

// 3. Các cấu hình mặc định khác
builder.Services.AddControllersWithViews();
builder.Services.AddLocalization(opt => opt.ResourcesPath = "Resources");

var app = builder.Build();

// --- KÍCH HOẠT ĐỊNH DẠNG TIẾNG VIỆT ---
app.UseRequestLocalization();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Student}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
