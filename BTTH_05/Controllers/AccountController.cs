using Microsoft.AspNetCore.Mvc;
using BTTH_05.Models;
using System.Linq;

namespace BTTH_05.Controllers
{
    public class AccountController : Controller
    {
        private readonly QuanLySinhVienDContext _context;

        public AccountController(QuanLySinhVienDContext context)
        {
            _context = context;
        }

        // 1. Hàm hiển thị giao diện đăng nhập
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // 2. Xử lý logic kiểm tra tài khoản khi Đăng nhập
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginViewModel model)
        {
            // Nếu để trống ô, hệ thống tự động bắn lỗi đỏ tại giao diện
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            bool isValidUser = false;

            // KIỂM TRA CỐ ĐỊNH: Tài khoản: Admin | Mật khẩu: dnu
            if (model.Username == "Admin" && model.Password == "dnu")
            {
                isValidUser = true;
            }
            else
            {
                // Kiểm tra tài khoản trong bảng TaiKhoans của Cơ sở dữ liệu
                var account = _context.TaiKhoans.FirstOrDefault(t =>
                    t.Username == model.Username &&
                    t.Password == model.Password);

                if (account != null) isValidUser = true;
            }

            // XỬ LÝ ĐIỀU HƯỚNG KHI ĐĂNG NHẬP THÀNH CÔNG
            if (isValidUser)
            {
                return RedirectToAction("Index", "Home");
            }

            // Nếu nhập sai thông tin, báo lỗi tổng dưới chân form
            ModelState.AddModelError(string.Empty, "Tài khoản hoặc mật khẩu không chính xác!");
            return View(model);
        }

        // 3. Hiển thị giao diện Đăng ký tài khoản (Khi nhấn nút Đăng ký ở trang Login)
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // 4. Xử lý logic lưu tài khoản mới vào Database khi Đăng ký
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(string username, string password, string confirmPassword)
        {
            // Kiểm tra dữ liệu đầu vào xem có bị bỏ trống hay không
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError(string.Empty, "Vui lòng điền đầy đủ thông tin!");
                return View();
            }

            // Kiểm tra mật khẩu và xác nhận mật khẩu có khớp nhau không
            if (password != confirmPassword)
            {
                ModelState.AddModelError(string.Empty, "Mật khẩu xác nhận không khớp!");
                return View();
            }

            // Kiểm tra xem tên tài khoản này đã có ai đăng ký trong database chưa
            var checkExist = _context.TaiKhoans.FirstOrDefault(t => t.Username == username);
            if (checkExist != null || username == "Admin")
            {
                ModelState.AddModelError(string.Empty, "Tên tài khoản này đã tồn tại trên hệ thống!");
                return View();
            }

            try
            {
                // Khởi tạo đối tượng tài khoản mới để lưu vào CSDL
                var newAccount = new TaiKhoan
                {
                    Username = username,
                    Password = password
                    // Nếu bảng TaiKhoan của cậu có thêm trường gì như Quyen, TenNguoiDung... cậu có thể gán thêm ở đây
                };

                _context.TaiKhoans.Add(newAccount);
                _context.SaveChanges(); // Lưu thay đổi vào SQL Server

                // Đăng ký thành công -> Điều hướng người dùng quay lại trang Đăng nhập
                return RedirectToAction("Login");
            }
            catch
            {
                ModelState.AddModelError(string.Empty, "Đã xảy ra lỗi trong quá trình tạo tài khoản. Vui lòng thử lại!");
                return View();
            }
        }

        // 5. Hàm Đăng xuất
        public IActionResult Logout()
        {
            return RedirectToAction("Login");
        }
    }
}
