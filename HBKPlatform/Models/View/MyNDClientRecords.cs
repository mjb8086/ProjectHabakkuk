using HBKPlatform.Models.DTO;

namespace HBKPlatform.Models.View;

public class MyNDClientRecords
{
    public string ClientName { get; set; }
    public int ClientId { get; set; }
    public List<ClientRecordLite> ClientRecords { get; set; }
}
