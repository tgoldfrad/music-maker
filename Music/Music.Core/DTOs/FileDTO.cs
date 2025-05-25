using Music.Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Music.Core.DTOs
{
    public class FileDTO
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public int CreatedBy { get; set; }

        public string Name { get; set; }

        public string Path { get; set; }

        public string Type { get; set; }

        public decimal Size { get; set; }
        public bool IsUser { get; set; }
        //public bool IsDeleted { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

    }
}
