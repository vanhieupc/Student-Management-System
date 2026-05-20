using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BTTH_05.Models
{
    [Table("ChuyenNganh")]
    public partial class ChuyenNganh
    {
        [Key]
        [Column("MaCN")]
        public int MaCN { get; set; }

        [Column("TenCN")]
        public string TenCN { get; set; } = null!;

        // KHÔNG ĐƯỢC để dòng ICollection<GiangVien> ở đây
    }
}
