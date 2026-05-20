using BTTH_05.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System;

namespace BTTH_05.Services
{
    public interface IStudentService
    {
        List<Student> GetAllStudents();
        Student? GetStudentById(long id);
        void AddStudent(Student student);
        void UpdateStudent(Student student);
        void DeleteStudent(long id);
        List<Student> SearchStudents(string name, string major);
    }

    public class StudentService : IStudentService
    {
        private readonly QuanLySinhVienDContext _context;

        public StudentService(QuanLySinhVienDContext context)
        {
            _context = context;
        }

        public List<Student> GetAllStudents()
        {
            return _context.Students
                           .Include(s => s.ChuyenNganh)
                           .OrderBy(s => s.MaSV)
                           .ToList();
        }

        public Student? GetStudentById(long id)
        {
            return _context.Students
                           .Include(s => s.ChuyenNganh)
                           .FirstOrDefault(s => s.MaSV == id);
        }

        public void AddStudent(Student student)
        {
            if (student == null) return;

            var isExist = _context.Students.Any(s => s.MaSV == student.MaSV);
            if (isExist)
            {
                throw new Exception("Mã sinh viên này đã tồn tại trong hệ thống!");
            }

            _context.Students.Add(student);
            _context.SaveChanges();
        }

        public void UpdateStudent(Student student)
        {
            if (student == null) return;
            _context.Students.Update(student);
            _context.SaveChanges();
        }

        public void DeleteStudent(long id)
        {
            var student = _context.Students.Find(id);
            if (student != null)
            {
                _context.Students.Remove(student);
                _context.SaveChanges();
            }
        }

        public List<Student> SearchStudents(string name, string major)
        {
            var query = _context.Students.Include(s => s.ChuyenNganh).AsQueryable();

            // Tìm kiếm theo Tên
            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(s => s.HoTen != null && s.HoTen.Contains(name));
            }

            // CẬP NHẬT: Tìm kiếm theo Ngành học hoặc Chuyên ngành
            if (!string.IsNullOrEmpty(major))
            {
                if (int.TryParse(major, out int majorId))
                {
                    // Nếu major truyền vào là ID (số)
                    query = query.Where(s => s.MaCN == majorId ||
                                           (s.TenCN_TuNhap != null && s.TenCN_TuNhap.Contains(major)) ||
                                           (s.NganhHoc != null && s.NganhHoc.Contains(major)));
                }
                else
                {
                    // Nếu major truyền vào là chuỗi văn bản (tên Ngành/Chuyên ngành)
                    query = query.Where(s => (s.TenCN_TuNhap != null && s.TenCN_TuNhap.Contains(major)) ||
                                           (s.NganhHoc != null && s.NganhHoc.Contains(major)));
                }
            }

            return query.OrderBy(s => s.MaSV).ToList();
        }
    }
}
