using BTTH_05.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BTTH_05.Services
{
    public class DiemService : IDiemService
    {
        private readonly QuanLySinhVienDContext _context;
        public DiemService(QuanLySinhVienDContext context) { _context = context; }

        public async Task<IEnumerable<Diem>> GetAllDiemsAsync() =>
            await _context.Diems
                .Include(d => d.Student)
                .Include(d => d.MonHoc)
                .ToListAsync();

        public async Task<IEnumerable<Diem>> GetSapTotNghiepAsync() =>
            await _context.Diems
                .Include(d => d.Student)
                .Include(d => d.MonHoc)
                .Where(d => d.Student != null && (d.Student.NamThu >= 4 || d.Student.TrangThai == "Sắp tốt nghiệp"))
                .ToListAsync();

        public async Task<Diem?> GetDiemByIdAsync(long maSV, string maMH) =>
            await _context.Diems
                .Include(d => d.Student)
                .Include(d => d.MonHoc)
                .FirstOrDefaultAsync(m => m.MaSV == maSV && m.MaMH == maMH);

        public double TinhDiemTrungBinh(double cc, double gk, double ck) =>
            Math.Round((cc * 0.1) + (gk * 0.3) + (ck * 0.6), 1);

        public void TinhDiemTrungBinh(Diem diem)
        {
            if (diem != null)
            {
                double cc = diem.DiemChuyenCan ?? 0.0;
                double gk = diem.DiemGiuaKy ?? 0.0;
                double ck = diem.DiemCuoiKy ?? 0.0;
                diem.DiemTrungBinh = TinhDiemTrungBinh(cc, gk, ck);
            }
        }

        // --- CẬP NHẬT: Fix lỗi trùng khóa chính (Primary Key) ---
        public async Task AddDiemAsync(Diem diem)
        {
            // Kiểm tra xem cặp MaSV và MaMH này đã tồn tại chưa trước khi thêm
            var checkTonTai = await _context.Diems
                .AnyAsync(d => d.MaSV == diem.MaSV && d.MaMH == diem.MaMH);

            if (checkTonTai)
            {
                throw new Exception("Sinh viên này đã có điểm môn học này rồi! Vui lòng dùng chức năng Sửa.");
            }

            TinhDiemTrungBinh(diem);
            _context.Diems.Add(diem);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateDiemAsync(Diem diem)
        {
            TinhDiemTrungBinh(diem);
            _context.Entry(diem).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteDiemAsync(long maSV, string maMH)
        {
            var item = await _context.Diems.FirstOrDefaultAsync(m => m.MaSV == maSV && m.MaMH == maMH);
            if (item != null)
            {
                _context.Diems.Remove(item);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<dynamic> GetDashboardStatsAsync()
        {
            var tongSV = await _context.Students.CountAsync();
            var sapTN = await _context.Students.CountAsync(s => s.NamThu >= 4 || s.TrangThai == "Sắp tốt nghiệp");
            var allDiem = await _context.Diems.ToListAsync();

            var scores = allDiem
                .Where(d => d.DiemTrungBinh.HasValue)
                .Select(d => d.DiemTrungBinh!.Value)
                .ToList();

            double gpa = scores.Any() ? scores.Average() : 0.0;
            var svXuatSac = allDiem.Count(d => d.DiemTrungBinh.HasValue && d.DiemTrungBinh.Value >= 8.0);

            return new
            {
                TongSV = tongSV,
                SapTotNghiepCount = sapTN,
                GPATrungBinh = Math.Round(gpa, 2),
                SinhVienXuatSac = svXuatSac
            };
        }

        public async Task<double> GetGPATrungBinhChungAsync()
        {
            var data = await _context.Diems.ToListAsync();
            var scores = data.Where(s => s.DiemTrungBinh.HasValue).Select(s => s.DiemTrungBinh!.Value).ToList();
            return scores.Any() ? Math.Round(scores.Average(), 2) : 0.0;
        }

        public async Task<int> GetSVXuatSacCountAsync()
        {
            var data = await _context.Diems.ToListAsync();
            return data.Count(d => d.DiemTrungBinh.HasValue && d.DiemTrungBinh.Value >= 8.0);
        }
    }
}
