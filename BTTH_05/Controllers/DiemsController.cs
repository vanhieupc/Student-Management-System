using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using BTTH_05.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace BTTH_05.Controllers
{
    public class DiemsController : Controller
    {
        private readonly QuanLySinhVienDContext _context;

        public DiemsController(QuanLySinhVienDContext context)
        {
            _context = context;
        }

        // 1. DANH SÁCH LỘ TRÌNH ĐIỂM
        public async Task<IActionResult> Index()
        {
            // Bốc thẳng từ bảng Student vì điểm đã nằm trong này rồi
            var students = await _context.Students.AsNoTracking().ToListAsync();
            return View(students);
        }

        // 2. NHẬP ĐIỂM MỚI (Dành cho việc tạo mới lộ trình)
        public IActionResult Create()
        {
            ViewData["MaSV"] = new SelectList(_context.Students, "MaSV", "HoTen");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(long MaSV, double? g1, double? g2, double? g3, double? g4)
        {
            var student = await _context.Students.FindAsync(MaSV);
            if (student == null) return NotFound();

            student.GpaNam1 = g1;
            student.GpaNam2 = g2;
            student.GpaNam3 = g3;
            student.GpaNam4 = g4;

            // Tính toán GPA tổng kết tự động dựa trên số năm có điểm
            double tong = 0; int count = 0;
            if (g1.HasValue && g1 > 0) { tong += g1.Value; count++; }
            if (g2.HasValue && g2 > 0) { tong += g2.Value; count++; }
            if (g3.HasValue && g3 > 0) { tong += g3.Value; count++; }
            if (g4.HasValue && g4 > 0) { tong += g4.Value; count++; }

            student.GpaTuNhap = count > 0 ? Math.Round(tong / count, 2) : 0;

            // Tự động đẩy trạng thái nếu đã có điểm năm cuối
            if (g4.HasValue && g4 > 0) student.TrangThai = "Sắp tốt nghiệp";

            _context.Update(student);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // 3. CHỈNH SỬA THÔNG TIN & ĐIỂM (Hàm này cực quan trọng)
        public async Task<IActionResult> Edit(long id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null) return NotFound();

            return View(student);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long MaSV, string HoTen, string LopHoc, int? NamThu, string TrangThai, double? GpaNam1, double? GpaNam2, double? GpaNam3, double? GpaNam4)
        {
            // Tìm sinh viên trong DB dựa trên MaSV từ form gửi lên
            var student = await _context.Students.FindAsync(MaSV);
            if (student == null) return NotFound();

            try
            {
                // Cập nhật thông tin cá nhân
                student.HoTen = HoTen;
                student.LopHoc = LopHoc;
                student.NamThu = NamThu;
                student.TrangThai = TrangThai;

                // Cập nhật điểm các năm
                student.GpaNam1 = GpaNam1;
                student.GpaNam2 = GpaNam2;
                student.GpaNam3 = GpaNam3;
                student.GpaNam4 = GpaNam4;

                // Tính toán lại GPA tích lũy
                double tong = 0; int count = 0;
                if (GpaNam1.HasValue && GpaNam1 > 0) { tong += GpaNam1.Value; count++; }
                if (GpaNam2.HasValue && GpaNam2 > 0) { tong += GpaNam2.Value; count++; }
                if (GpaNam3.HasValue && GpaNam3 > 0) { tong += GpaNam3.Value; count++; }
                if (GpaNam4.HasValue && GpaNam4 > 0) { tong += GpaNam4.Value; count++; }

                student.GpaTuNhap = count > 0 ? Math.Round(tong / count, 2) : 0;

                _context.Update(student);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Cập nhật dữ liệu thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Lỗi khi lưu: " + ex.Message);
                return View(student);
            }
        }

        // 4. RESET ĐIỂM (Xóa trắng điểm về 0)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetDiem(long id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student != null)
            {
                student.GpaNam1 = null;
                student.GpaNam2 = null;
                student.GpaNam3 = null;
                student.GpaNam4 = null;
                student.GpaTuNhap = 0;
                _context.Update(student);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
