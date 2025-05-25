using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Music.Api.PostModels
{
    public class FilePostModel
    {
        public int CreatedBy { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public string Type { get; set; }
        public decimal Size { get; set; }
        public bool IsUser { get; set; }
    }
}
