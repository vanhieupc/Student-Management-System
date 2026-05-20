using BTTH_05.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BTTH_05.Services
{
    public interface IDiemService
    {
        // Hàm này cực kỳ quan trọng để lấy bảng điểm 4 năm
        Task<IEnumerable<Diem>> GetAllDiemsAsync();

        Task<Diem?> GetDiemByIdAsync(long maSV, string maMH);
        Task<IEnumerable<Diem>> GetSapTotNghiepAsync();
        Task AddDiemAsync(Diem diem);
        Task UpdateDiemAsync(Diem diem);
        Task DeleteDiemAsync(long maSV, string maMH);

        // Các hàm tính toán logic
        double TinhDiemTrungBinh(double cc, double gk, double ck);
        void TinhDiemTrungBinh(Diem diem);

        Task<dynamic> GetDashboardStatsAsync();

        // Thống kê cho Dashboard
        Task<double> GetGPATrungBinhChungAsync();
        Task<int> GetSVXuatSacCountAsync();

        // Nếu ông muốn lấy điểm riêng cho 1 sinh viên nhanh hơn, có thể thêm hàm này (tùy chọn)
        // Task<IEnumerable<Diem>> GetDiemByStudentIdAsync(long maSV);
    }
}
