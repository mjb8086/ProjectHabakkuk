namespace HBKPlatform.Models.View.MCP
{
    public struct ListClinics
    {
        public List<ClinicDetailsLite> Clinics { get; set; }
    }

    public struct ClinicDetailsLite
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}