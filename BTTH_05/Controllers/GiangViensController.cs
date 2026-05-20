using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BTTH_05.Models;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace BTTH_05.Controllers
{
    public class GiangViensController : Controller
    {
        private readonly QuanLySinhVienDContext _context;

        public GiangViensController(QuanLySinhVienDContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var danhSach = await _context.GiangViens.AsNoTracking().ToListAsync();
            ViewBag.DanhSachKhoa = danhSach
                .Select(g => g.Khoa)
                .Where(k => !string.IsNullOrEmpty(k))
                .Distinct()
                .OrderBy(k => k)
                .ToList();
            return View(danhSach);
        }

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaGV,HoTen,Email,MaCN,SoDienThoai,Khoa,ChucDanh,TrangThai")] GiangVien giangVien)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(giangVien);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Mã GV này Quân nhập bị trùng rồi.");
                }
            }
            return View(giangVien);
        }

        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();
            // Lấy dữ liệu gốc để đổ lên Form
            var giangVien = await _context.GiangViens.AsNoTracking().FirstOrDefaultAsync(g => g.MaGV == id);
            if (giangVien == null) return NotFound();
            return View(giangVien);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string oldId, [Bind("MaGV,HoTen,Email,MaCN,SoDienThoai,Khoa,ChucDanh,TrangThai")] GiangVien giangVien)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // 1. Tìm bản ghi gốc TRONG database (không dùng AsNoTracking để chỉnh sửa trực tiếp)
                    var banGhiCu = await _context.GiangViens.FirstOrDefaultAsync(g => g.MaGV == oldId);
                    if (banGhiCu == null) return NotFound();

                    if (oldId != giangVien.MaGV)
                    {
                        // 2. TRƯỜNG HỢP ĐỔI MÃ: Xóa cũ, thêm mới
                        if (await _context.GiangViens.AnyAsync(g => g.MaGV == giangVien.MaGV))
                        {
                            ModelState.AddModelError("MaGV", "Mã mới này trùng với người khác rồi nhé.");
                            return View(giangVien);
                        }

                        _context.GiangViens.Remove(banGhiCu);
                        await _context.SaveChangesAsync();

                        _context.GiangViens.Add(giangVien);
                    }
                    else
                    {
                        // 3. TRƯỜNG HỢP KHÔNG ĐỔI MÃ: Ép cập nhật từng trường một (Bao gồm Trạng thái)
                        banGhiCu.HoTen = giangVien.HoTen;
                        banGhiCu.Email = giangVien.Email;
                        banGhiCu.SoDienThoai = giangVien.SoDienThoai;
                        banGhiCu.Khoa = giangVien.Khoa;
                        banGhiCu.ChucDanh = giangVien.ChucDanh;
                        banGhiCu.TrangThai = giangVien.TrangThai; // Ép gán "Nghỉ phép" ở đây

                        // Nếu MaCN gửi về bị null thì giữ lại cái cũ
                        if (giangVien.MaCN != null) banGhiCu.MaCN = giangVien.MaCN;

                        _context.Update(banGhiCu);
                    }

                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Lỗi cập nhật: " + ex.Message);
                }
            }
            return View(giangVien);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();
            var giangVien = await _context.GiangViens.FindAsync(id);
            if (giangVien != null)
            {
                _context.GiangViens.Remove(giangVien);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
