using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Music.Api.PostModels
{
    public enum ConversionType { NoteToAudio = 1, AoudioToNote = 2 }
    public class ConversionProcessPostModel
    {
        public int SourceFile { get; set; }
        public ConversionType ConversionType { get; set; }
        //public int? ResultFile { get; set; }

    }
}
