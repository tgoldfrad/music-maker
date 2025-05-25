using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Music.Core.Entities
{
    [Table("Files")]
    public class File
    {
        [Key]
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public int CreatedBy { get; set; }
        [ForeignKey(nameof(CreatedBy))]
        public User User { get; set; }
        [MaxLength(255)]
        public string Name { get; set; }
        [MaxLength(500)]
        public string Path { get; set; }
        [MaxLength(50)]
        public string Type { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal Size { get; set; }
        public bool IsUser { get; set; }
        //public bool IsDeleted { get; set; } = false;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;


    }
}
