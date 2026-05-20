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
        private const int PageSize = 6; // Đã chỉnh thành 6 để khớp với Layout 3 cột

        public StudentController(IStudentService studentService, IDiemService diemService)
        {
            _studentService = studentService;
            _diemService = diemService;
        }

        // 1. Danh sách sinh viên - Hiển thị Card và Thống kê
        public IActionResult Index(int page = 1, string searchName = "", string searchMajor = "")
        {
            // Lấy danh sách đã qua lọc tìm kiếm (Tên, MSSV hoặc Chuyên ngành)
            var allFilteredStudents = _studentService.SearchStudents(searchName, searchMajor);

            // Lấy toàn bộ danh sách để tính toán số liệu cho 5 card thống kê ở Header
            var fullList = _studentService.GetAllStudents();

            // Đổ dữ liệu vào ViewBag cho Header
            ViewBag.TotalStudents = fullList.Count;
            ViewBag.Year4Students = fullList.Count(s => s.TrangThai == "Sắp tốt nghiệp" || s.NamThu == 4);
            ViewBag.ActiveStudents = fullList.Count(s => s.TrangThai == "Đang học");
            ViewBag.BaoLuuStudents = fullList.Count(s => s.TrangThai == "Bảo lưu");
            ViewBag.NghiHocStudents = fullList.Count(s => s.TrangThai == "Nghỉ học");

            // Tính toán phân trang
            var totalItems = allFilteredStudents.Count;
            var totalPages = (int)Math.Ceiling((double)totalItems / PageSize);

            if (page < 1) page = 1;
            if (totalPages > 0 && page > totalPages) page = totalPages;

            var pagedStudents = allFilteredStudents
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            // Khởi tạo ViewModel đồng bộ với file StudentListViewModel.cs Quân vừa sửa
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

        // 2. Danh sách sắp tốt nghiệp (Dành cho chức năng thống kê riêng)
        public async Task<IActionResult> SapTotNghiep()
        {
            var fullList = _studentService.GetAllStudents();

            var listSapTotNghiep = fullList
                .Where(s => s.TrangThai == "Sắp tốt nghiệp" || s.NamThu == 4)
                .ToList();

            var allDiems = await _diemService.GetAllDiemsAsync();
            var gpaDictionary = new Dictionary<long, double>();

            foreach (var sv in listSapTotNghiep)
            {
                if (sv.GpaTuNhap.HasValue && sv.GpaTuNhap > 0)
                {
                    gpaDictionary[sv.MaSV] = sv.GpaTuNhap.Value;
                }
                else
                {
                    var diemCuaSv = allDiems.Where(d => d.MaSV == sv.MaSV).ToList();
                    double gpa = diemCuaSv.Any() ? Math.Round(diemCuaSv.Average(d => d.DiemTrungBinh ?? 0), 2) : 0;
                    gpaDictionary[sv.MaSV] = gpa;
                }
            }

            ViewBag.GpaList = gpaDictionary;
            return View(listSapTotNghiep);
        }

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

        public async Task<IActionResult> Details(long id)
        {
            var student = _studentService.GetStudentById(id);
            if (student == null) return NotFound();

            var allDiems = await _diemService.GetAllDiemsAsync();
            var diemList = allDiems.Where(d => d.MaSV == id).ToList();

            ViewBag.BangDiem = diemList;

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
