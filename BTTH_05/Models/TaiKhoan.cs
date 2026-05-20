using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BTTH_05.Models
{
    [Table("TaiKhoan")]
    public class TaiKhoan
    {
        [Key]
        [Column("Username")]
        [Display(Name = "Tên đăng nhập")]
        public string Username { get; set; } = string.Empty;

        [Column("Password")]
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; } = string.Empty;

        [Column("HoTen")]
        [Display(Name = "Họ tên")]
        public string? HoTen { get; set; }

        [Column("Role")]
        public string? Role { get; set; }
    }
}
