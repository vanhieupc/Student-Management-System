using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BTTH_05.Models
{
    [Table("SinhVien")]
    public class Student
    {
        // Hàm khởi tạo constructor để tránh lỗi Null danh sách liên kết
        public Student()
        {
            Diems = new HashSet<Diem>();
        }

        [Key]
        [Column("MaSV")]
        [Display(Name = "Mã Sinh Viên")]
        [Required(ErrorMessage = "Mã sinh viên không được để trống")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long MaSV { get; set; }

        [Required(ErrorMessage = "Họ tên không được để trống")]
        [Column("HoTen")]
        [Display(Name = "Họ và Tên")]
        public string HoTen { get; set; } = string.Empty;

        // --- ĐÃ TỐI ƯU: Thêm bắt lỗi định dạng Email chuẩn quy tắc ---
        [Column("Email")]
        [Display(Name = "Địa chỉ Email")]
        [EmailAddress(ErrorMessage = "Địa chỉ Email không đúng định dạng")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Email không hợp lệ (Ví dụ: sv@dainam.edu.vn)")]
        public string? Email { get; set; }

        [Column("Sdt")]
        [Display(Name = "Số điện thoại")]
        [RegularExpression(@"^(0[3|5|7|8|9])+([0-9]{8})$", ErrorMessage = "Số điện thoại không đúng định dạng (phải gồm 10 số)")]
        public string? Sdt { get; set; }

        [Column("LopHoc")]
        [Display(Name = "Lớp học")]
        public string? LopHoc { get; set; }

        [Column("NamThu")]
        [Display(Name = "Năm thứ")]
        [Range(1, 5, ErrorMessage = "Năm học phải nằm trong khoảng từ 1 đến 5")]
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
        [Display(Name = "Ngày sinh")]
        public DateTime? NgaySinh { get; set; }

        [Column("NamHoc")]
        public int? NamHoc { get; set; }

        [Column("TrangThai")]
        [Display(Name = "Trạng thái học tập")]
        public string? TrangThai { get; set; }

        // --- NHÓM GPA ---
        [Column("GpaTuNhap")]
        [Display(Name = "GPA Tích lũy")]
        [DisplayFormat(DataFormatString = "{0:0.00}", NullDisplayText = "0.00")]
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

        public virtual ICollection<Diem> Diems { get; set; }
    }
}
