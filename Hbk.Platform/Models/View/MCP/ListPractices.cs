namespace Hbk.Platform.Models.View.MCP
{
    public struct ListPractices
    {
        public List<PracticeDetailsLite> Practices { get; set; }
    }

    public struct PracticeDetailsLite
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}