using System;
using System.Collections.Generic;

namespace BTTH_05.Models
{
    public class StudentListViewModel
    {
        // Danh sách sinh viên hiển thị trên trang hiện tại
        // Dùng IEnumerable để tối ưu hiệu suất khi truy vấn dữ liệu
        public IEnumerable<Student> Students { get; set; } = new List<Student>();

        // Thông tin phân trang
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public int PageSize { get; set; } = 6; // Đổi thành 6 để hiển thị 2 hàng x 3 cột cho đẹp

        // Thông tin tìm kiếm
        public string? SearchMajor { get; set; }
        public string? SearchName { get; set; }

        // Bổ sung thêm các thuộc tính thống kê nếu Quân muốn đẩy trực tiếp qua Model thay vì ViewBag (Tùy chọn)
        public int TotalStudents { get; set; }
        public int ActiveStudents { get; set; }
        public int GraduatingSoonStudents { get; set; }
    }
}
