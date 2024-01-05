using HBKPlatform.Globals;

namespace HBKPlatform.Models.DTO;

public class ClientDetailsLite
{
    public string Name { get; set; }
    public int Id { get; set; }
    public int ClinicId { get; set; }
}

public class PracDetailsLite
{
    public string Name { get; set; }
    public int Id { get; set; }
    public int ClinicId { get; set; }
}

public class ClientRecordLite
{
    public int Id { get; set; }
    public string ClientName { get; set; }
    public string Title { get; set; }
    public DateTime Date { get; set; }
    public Enums.RecordVisibility Visibility { get; set; }
}