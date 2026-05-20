using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BTTH_05.Models
{
    [Table("SinhVien")]
    public class Student
    {
        [Key]
        [Column("MaSV")]
        [Display(Name = "Mã Sinh Viên")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long MaSV { get; set; }

        [Required(ErrorMessage = "Họ tên không được để trống")]
        [Column("HoTen")]
        [Display(Name = "Họ và Tên")]
        public string HoTen { get; set; } = string.Empty;

        [Column("Email")]
        public string? Email { get; set; }

        [Column("Sdt")]
        [Display(Name = "Số điện thoại")]
        public string? Sdt { get; set; }

        [Column("LopHoc")]
        public string? LopHoc { get; set; }

        [Column("NamThu")]
        public int? NamThu { get; set; }

        // --- BỔ SUNG: NGÀNH HỌC ---
        [Column("NganhHoc")]
        [Display(Name = "Ngành Học")]
        public string? NganhHoc { get; set; }

        [Column("TenMonHoc")]
        public string? TenMonHoc { get; set; }

        [Column("TenCN_TuNhap")]
        [Display(Name = "Chuyên Ngành")]
        public string? TenCN_TuNhap { get; set; }

        // --- ĐÃ CẬP NHẬT ĐỊNH DẠNG NGÀY/THÁNG/NĂM ---
        [Column("NgaySinh")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? NgaySinh { get; set; }

        [Column("NamHoc")]
        public int? NamHoc { get; set; }

        [Column("TrangThai")]
        public string? TrangThai { get; set; }

        // --- NHÓM GPA ---
        [Column("GpaTuNhap")]
        [Display(Name = "GPA Tích lũy")]
        [DisplayFormat(DataFormatString = "{0:0.00}")]
        public double? GpaTuNhap { get; set; }

        [Column("GpaNam1")]
        [Display(Name = "GPA Năm 1")]
        public double? GpaNam1 { get; set; }

        [Column("GpaNam2")]
        [Display(Name = "GPA Năm 2")]
        public double? GpaNam2 { get; set; }

        [Column("GpaNam3")]
        [Display(Name = "GPA Năm 3")]
        public double? GpaNam3 { get; set; }

        [Column("GpaNam4")]
        [Display(Name = "GPA Năm 4")]
        public double? GpaNam4 { get; set; }

        [Column("MaCN")]
        public int? MaCN { get; set; }

        [ForeignKey("MaCN")]
        public virtual ChuyenNganh? ChuyenNganh { get; set; }

        public virtual ICollection<Diem>? Diems { get; set; }
    }
}
