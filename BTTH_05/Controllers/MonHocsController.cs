using BTTH_05.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BTTH_05.Controllers
{
    public class MonHocsController : Controller
    {
        private readonly QuanLySinhVienDContext _context;

        public MonHocsController(QuanLySinhVienDContext context)
        {
            _context = context;
        }

        // 1. TRANG DANH SÁCH - ĐÃ THÊM LOGIC PHÂN TRANG 6 CARD
        public async Task<IActionResult> Index(int page = 1)
        {
            const int PageSize = 6; // Quân muốn 6 card 1 trang cho đẹp

            // Lấy toàn bộ dữ liệu để làm bộ lọc và tính tổng trang
            var allMonHocs = await _context.MonHocs.AsNoTracking().ToListAsync();

            // Đổ danh sách khoa cho bộ lọc (giữ nguyên logic của Quân)
            ViewBag.DanhSachKhoa = allMonHocs
                .Select(m => m.Khoa)
                .Distinct()
                .Where(k => !string.IsNullOrEmpty(k))
                .OrderBy(k => k)
                .ToList();

            // Tính toán phân trang
            int totalItems = allMonHocs.Count;
            int totalPages = (int)Math.Ceiling((double)totalItems / PageSize);

            // Đảm bảo trang hiện tại không vượt quá giới hạn
            if (page < 1) page = 1;
            if (page > totalPages && totalPages > 0) page = totalPages;

            // Lấy đúng 6 bản ghi dựa trên số trang
            var pagedData = allMonHocs
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            // Truyền thông tin phân trang qua ViewBag để View hiển thị thanh số trang
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View(pagedData);
        }

        // 2. THÊM MỚI (Giao diện)
        public IActionResult Create() => View();

        // 3. THÊM MỚI (Xử lý)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaMH,TenMH,SoTinChi,HocKy,MaGV,Khoa,SiSoHienTai,SiSoToiDa,TrangThai")] MonHoc monHoc)
        {
            if (_context.MonHocs.Any(m => m.MaMH == monHoc.MaMH))
            {
                ModelState.AddModelError("MaMH", "Mã môn học này đã tồn tại rồi Quân ơi!");
            }

            if (ModelState.IsValid)
            {
                _context.Add(monHoc);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(monHoc);
        }

        // 4. SỬA (Giao diện)
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var monHoc = await _context.MonHocs.AsNoTracking().FirstOrDefaultAsync(m => m.MaMH == id);

            if (monHoc == null) return NotFound();
            return View(monHoc);
        }

        // 5. SỬA (Xử lý lưu dữ liệu)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("MaMH,TenMH,SoTinChi,HocKy,MaGV,Khoa,SiSoHienTai,SiSoToiDa,TrangThai")] MonHoc monHoc)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var banGhiCu = await _context.MonHocs.FirstOrDefaultAsync(m => m.MaMH == id);
                    if (banGhiCu == null) return NotFound();

                    if (id != monHoc.MaMH)
                    {
                        if (_context.MonHocs.Any(m => m.MaMH == monHoc.MaMH))
                        {
                            ModelState.AddModelError("MaMH", "Mã môn học mới này đã tồn tại!");
                            return View(monHoc);
                        }

                        _context.MonHocs.Remove(banGhiCu);
                        await _context.SaveChangesAsync();

                        _context.MonHocs.Add(monHoc);
                    }
                    else
                    {
                        _context.Entry(banGhiCu).CurrentValues.SetValues(monHoc);
                    }

                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Lỗi khi cập nhật dữ liệu: " + ex.Message);
                }
            }
            return View(monHoc);
        }

        // 6. XÓA
        [HttpPost] // Thêm HttpPost để an toàn hơn cho nút xóa của Quân
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var monHoc = await _context.MonHocs.FindAsync(id);
            if (monHoc != null)
            {
                _context.MonHocs.Remove(monHoc);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
