using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace BTTH_05.Models
{
    public partial class QuanLySinhVienDContext : DbContext
    {
        public QuanLySinhVienDContext() { }

        public QuanLySinhVienDContext(DbContextOptions<QuanLySinhVienDContext> options)
            : base(options) { }

        public virtual DbSet<ChuyenNganh> ChuyenNganhs { get; set; }
        public virtual DbSet<GiangVien> GiangViens { get; set; }
        public virtual DbSet<MonHoc> MonHocs { get; set; }
        public virtual DbSet<Student> Students { get; set; }
        public virtual DbSet<Diem> Diems { get; set; }
        public virtual DbSet<TaiKhoan> TaiKhoans { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=LAPTOP-933Q3QK0;Database=QuanLySinhVien_D;Trusted_Connection=True;TrustServerCertificate=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 1. Cấu hình bảng Chuyên Ngành
            modelBuilder.Entity<ChuyenNganh>(entity =>
            {
                entity.ToTable("ChuyenNganh");
                entity.HasKey(e => e.MaCN);
            });

            // 2. Cấu hình bảng Giảng Viên
            modelBuilder.Entity<GiangVien>(entity =>
            {
                entity.ToTable("GiangVien");
                entity.HasKey(e => e.MaGV);
                entity.Property(e => e.MaGV).HasMaxLength(250);
            });

            // 3. Cấu hình bảng Môn Học
            modelBuilder.Entity<MonHoc>(entity =>
            {
                entity.ToTable("MonHoc");
                entity.HasKey(e => e.MaMH);
            });

            // 4. Cấu hình bảng Sinh Viên (Student)
            modelBuilder.Entity<Student>(entity =>
            {
                entity.ToTable("SinhVien");
                entity.HasKey(e => e.MaSV);

                entity.Property(e => e.MaSV).HasColumnName("MaSV").ValueGeneratedNever();
                entity.Property(e => e.HoTen).HasColumnName("HoTen");
                entity.Property(e => e.MaCN).HasColumnName("MaCN");
                // Đảm bảo ánh xạ cột mới Quân vừa thêm trong SQL
                entity.Property(e => e.GpaTuNhap).HasColumnName("GpaTuNhap").HasColumnType("float");
            });

            // 5. Cấu hình bảng Điểm - PHẦN QUAN TRỌNG NHẤT ĐỂ FIX LỖI
            modelBuilder.Entity<Diem>(entity =>
            {
                entity.ToTable("Diem");

                // Khóa chính kết hợp
                entity.HasKey(e => new { e.MaSV, e.MaMH });

                entity.Property(e => e.DiemChuyenCan).HasColumnType("float");
                entity.Property(e => e.DiemGiuaKy).HasColumnType("float");
                entity.Property(e => e.DiemCuoiKy).HasColumnType("float");
                entity.Property(e => e.DiemTrungBinh).HasColumnType("float");

                // FIX LỖI StudentMaSV TẠI ĐÂY:
                entity.HasOne(d => d.Student)
                    .WithMany(s => s.Diems) // Phải có liên kết ngược lại
                    .HasForeignKey(d => d.MaSV) // Dùng MaSV làm khóa ngoại
                    .HasPrincipalKey(s => s.MaSV) // Chỉ định khóa chính của Student là MaSV
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.MonHoc)
                    .WithMany()
                    .HasForeignKey(d => d.MaMH)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            // 6. Cấu hình bảng Tài Khoản
            modelBuilder.Entity<TaiKhoan>(entity =>
            {
                entity.ToTable("TaiKhoan");
                entity.HasKey(e => e.Username);
            });
        }
    }
}
