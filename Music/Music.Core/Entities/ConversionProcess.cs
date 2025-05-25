using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Music.Core.Entities
{
    public enum ConversionType { NoteToAudio = 1, AoudioToNote = 2 }
    [Table("ConversionProcesses")]
    public class ConversionProcess
    {
        [Key]
        public int Id { get; set; }
        public int SourceFile { get; set; }
        [ForeignKey(nameof(SourceFile))]
        public File Source { get; set; }
        public ConversionType ConversionType { get; set; }
        public int? ResultFile { get; set; }
        [ForeignKey(nameof(ResultFile))]
        public File Result { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
