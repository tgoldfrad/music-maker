namespace Music.Api.PutModels
{
    public class FilePutModel
    {

        public string Name { get; set; }
        //public string Path { get; set; }
        public string Type { get; set; }
        public decimal Size { get; set; }
        public bool IsUser { get; set; }
    }
}
