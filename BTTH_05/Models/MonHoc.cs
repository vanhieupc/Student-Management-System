using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BTTH_05.Models
{
    [Table("MonHoc")]
    public class MonHoc
    {
        [Key]
        [Column("MaMH")]
        [Display(Name = "Mã môn học")]
        public string MaMH { get; set; } = string.Empty;

        [Column("TenMH")]
        [Required(ErrorMessage = "Tên môn học không được để trống")]
        [Display(Name = "Tên môn học")]
        public string TenMH { get; set; } = string.Empty;

        [Column("SoTinChi")]
        [Display(Name = "Số tín chỉ")]
        public int SoTinChi { get; set; }

        [Column("HocKy")]
        [Display(Name = "Học kỳ")]
        public string? HocKy { get; set; }

        // Đã cập nhật thành string để Quân gõ tên giảng viên thoải mái
        [Column("MaGV")]
        [Display(Name = "Giảng viên giảng dạy")]
        public string? MaGV { get; set; }

        [Column("Khoa")]
        [Display(Name = "Khoa")]
        public string? Khoa { get; set; }

        [Column("SiSoHienTai")]
        [Display(Name = "Sĩ số hiện tại")]
        public int? SiSoHienTai { get; set; }

        [Column("SiSoToiDa")]
        [Display(Name = "Sĩ số tối đa")]
        public int? SiSoToiDa { get; set; }

        [Column("TrangThai")]
        [Display(Name = "Trạng thái")]
        public string? TrangThai { get; set; }

        // Đã loại bỏ các khóa ngoại virtual để tránh lỗi xung đột kiểu dữ liệu
    }
}
