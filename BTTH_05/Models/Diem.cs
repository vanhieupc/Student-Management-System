using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BTTH_05.Models
{
    [Table("Diem")]
    public class Diem
    {
        // 1. KHÓA CHÍNH KẾT HỢP (Composite Key)
        // Lưu ý: Nếu trong Database ông không để khóa chính kết hợp, hãy bỏ Order đi và thêm một cột Id riêng.
        [Key, Column("MaSV", Order = 0)]
        [Display(Name = "Mã sinh viên")]
        public long MaSV { get; set; }

        [Key, Column("MaMH", Order = 1)]
        [Display(Name = "Mã môn học")]
        public string MaMH { get; set; } = string.Empty;

        // 2. CHI TIẾT ĐIỂM THÀNH PHẦN
        [Column("DiemChuyenCan")]
        [Display(Name = "Chuyên cần (10%)")]
        public double? DiemChuyenCan { get; set; }

        [Column("DiemGiuaKy")]
        [Display(Name = "Giữa kỳ (30%)")]
        public double? DiemGiuaKy { get; set; }

        [Column("DiemCuoiKy")]
        [Display(Name = "Cuối kỳ (60%)")]
        public double? DiemCuoiKy { get; set; }

        [Column("DiemTrungBinh")]
        [Display(Name = "GPA Hệ 10")]
        public double? DiemTrungBinh { get; set; }

        // 3. THÔNG TIN QUẢN LÝ
        [Column("HocKy")]
        [Display(Name = "Học kỳ")]
        public string? HocKy { get; set; }

        [Column("Lop")]
        [Display(Name = "Lớp học")]
        public string? Lop { get; set; }

        [Column("NamHoc")]
        [Display(Name = "Năm học thứ")]
        public int? NamHoc { get; set; }

        [Column("TrangThai")]
        [Display(Name = "Trạng thái môn học")]
        public string? TrangThai { get; set; } = "Đang học";

        // 4. CÁC THUỘC TÍNH TỰ ĐỘNG (Hiển thị UI)
        [NotMapped]
        public string DiemChu
        {
            get
            {
                if (!DiemTrungBinh.HasValue) return "---";
                if (DiemTrungBinh >= 8.5) return "A";
                if (DiemTrungBinh >= 7.0) return "B";
                if (DiemTrungBinh >= 5.5) return "C";
                if (DiemTrungBinh >= 4.0) return "D";
                return "F";
            }
        }

        [NotMapped]
        public string KetQua => (DiemTrungBinh >= 4.0) ? "Qua môn" : "Học lại";

        [NotMapped]
        public string MauKetQua => (DiemTrungBinh >= 4.0) ? "text-success" : "text-danger";

        // 5. CẤU HÌNH QUAN HỆ (Fix lỗi StudentMaSV)
        [ForeignKey(nameof(MaSV))]
        public virtual Student? Student { get; set; }

        [ForeignKey(nameof(MaMH))]
        public virtual MonHoc? MonHoc { get; set; }
    }
}
