using Microsoft.AspNetCore.Mvc;
using BTTH_05.Models;
using BTTH_05.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BTTH_05.Controllers
{
    public class StudentController : Controller
    {
        private readonly IStudentService _studentService;
        private readonly IDiemService _diemService;
        private const int PageSize = 6;

        public StudentController(IStudentService studentService, IDiemService diemService)
        {
            _studentService = studentService;
            _diemService = diemService;
        }

        // 1. Danh sách sinh viên - Index
        public IActionResult Index(int page = 1, string searchName = "", string searchMajor = "")
        {
            var allFilteredStudents = _studentService.SearchStudents(searchName, searchMajor);
            var fullList = _studentService.GetAllStudents();

            // Thống kê Header - Đã sửa đếm Sắp tốt nghiệp phải có GPA >= 5
            ViewBag.TotalStudents = fullList.Count;
            ViewBag.Year4Students = fullList.Count(s => (s.TrangThai == "Sắp tốt nghiệp" || s.NamThu == 4) && (s.GpaTuNhap >= 5.0));
            ViewBag.ActiveStudents = fullList.Count(s => s.TrangThai == "Đang học");
            ViewBag.BaoLuuStudents = fullList.Count(s => s.TrangThai == "Bảo lưu");
            ViewBag.NghiHocStudents = fullList.Count(s => s.TrangThai == "Nghỉ học");

            var totalItems = allFilteredStudents.Count;
            var totalPages = (int)Math.Ceiling((double)totalItems / PageSize);

            if (page < 1) page = 1;
            if (totalPages > 0 && page > totalPages) page = totalPages;

            var pagedStudents = allFilteredStudents
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            var viewModel = new StudentListViewModel
            {
                Students = pagedStudents,
                CurrentPage = page,
                TotalPages = totalPages,
                PageSize = PageSize,
                SearchName = searchName,
                SearchMajor = searchMajor
            };

            return View(viewModel);
        }

        // 2. Danh sách sắp tốt nghiệp - Chỉ lấy người ĐỦ ĐIỀU KIỆN (GPA >= 5)
        public async Task<IActionResult> SapTotNghiep()
        {
            // Lấy toàn bộ SV từ Service
            var fullList = _studentService.GetAllStudents();

            // Lọc danh sách: Năm 4/Sắp tốt nghiệp VÀ điểm GPA tự nhập phải >= 5.0
            var listSapTotNghiep = fullList
                .Where(s => (s.TrangThai == "Sắp tốt nghiệp" || s.NamThu == 4) && s.GpaTuNhap >= 5.0)
                .OrderByDescending(s => s.GpaTuNhap)
                .ToList();

            // Trả về View trực tiếp danh sách này
            return View(listSapTotNghiep);
        }

        // 3. Thêm mới sinh viên
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("MaSV,HoTen,Email,Sdt,TrangThai,MaCN,NamThu,NamHoc,LopHoc,NgaySinh,TenCN_TuNhap,TenMonHoc,GpaTuNhap,NganhHoc")] Student student)
        {
            try
            {
                _studentService.AddStudent(student);
                TempData["SuccessMessage"] = $"Thêm thành công sinh viên: {student.HoTen}";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Lỗi khi thêm: " + ex.Message);
                return View(student);
            }
        }

        // 4. Chi tiết sinh viên
        public async Task<IActionResult> Details(long id)
        {
            var student = _studentService.GetStudentById(id);
            if (student == null) return NotFound();

            var allDiems = await _diemService.GetAllDiemsAsync();
            var diemList = allDiems.Where(d => d.MaSV == id).ToList();

            ViewBag.BangDiem = diemList;

            // Tính toán GPA hiển thị trong Details
            if (student.GpaTuNhap.HasValue && student.GpaTuNhap > 0)
            {
                ViewBag.GPATichLuy = student.GpaTuNhap.Value;
            }
            else
            {
                ViewBag.GPATichLuy = diemList.Any() ? Math.Round(diemList.Average(d => d.DiemTrungBinh ?? 0), 2) : 0;
            }

            return View(student);
        }

        // 5. Chỉnh sửa thông tin
        public IActionResult Edit(long id)
        {
            var student = _studentService.GetStudentById(id);
            if (student == null) return NotFound();
            return View(student);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(long id, [Bind("MaSV,HoTen,Email,Sdt,TrangThai,MaCN,NamThu,NamHoc,LopHoc,NgaySinh,TenCN_TuNhap,TenMonHoc,GpaTuNhap,NganhHoc")] Student student)
        {
            if (id != student.MaSV) return NotFound();

            try
            {
                _studentService.UpdateStudent(student);
                TempData["SuccessMessage"] = "Cập nhật thông tin thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Lỗi cập nhật: " + ex.Message);
                return View(student);
            }
        }

        // 6. Xóa sinh viên
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(long id)
        {
            var student = _studentService.GetStudentById(id);
            if (student == null) return NotFound();

            try
            {
                _studentService.DeleteStudent(id);
                TempData["SuccessMessage"] = $"Đã xóa sinh viên: {student.HoTen}";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Không thể xóa: " + ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
