namespace DMSRAG.Web.Data
{
    
    public class RAGItem
    {
        public List<SourceItem> Sources { get; set; } = new();
        public string Question { get; set; }
        public string Answer { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class SourceItem
    {
        public string Source { get; set; }
        public string Link { get; set; }
    }
}
