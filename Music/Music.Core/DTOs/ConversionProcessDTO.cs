using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Music.Core.DTOs
{
    public enum ConversionType { NoteToAudio = 1, AoudioToNote = 2 }
    public class ConversionProcessDTO
    {
        public int Id { get; set; }
        public int SourceFile { get; set; }
        public ConversionType ConversionType { get; set; }
        public int? ResultFile { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
