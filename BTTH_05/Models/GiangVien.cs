using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BTTH_05.Models
{
    [Table("GiangVien")]
    public class GiangVien
    {
        [Key]
        [Column("MaGV")]
        [Display(Name = "Mã giảng viên")]
        // TRONG SQL LÀ NVARCHAR NÊN Ở ĐÂY PHẢI LÀ STRING
        public string MaGV { get; set; } = string.Empty;

        [Column("HoTen")]
        [Display(Name = "Họ tên giảng viên")]
        [Required(ErrorMessage = "Không được để trống họ tên")]
        public string HoTen { get; set; } = string.Empty;

        [Column("Email")]
        public string? Email { get; set; }

        [Column("MaCN")]
        public int? MaCN { get; set; } // Trong ảnh SQL của bạn MaCN là kiểu int

        [Column("SoDienThoai")]
        public string? SoDienThoai { get; set; }

        [Column("Khoa")]
        public string? Khoa { get; set; }

        [Column("ChucDanh")]
        public string? ChucDanh { get; set; }

        [Column("TrangThai")]
        public string? TrangThai { get; set; }
    }
}
