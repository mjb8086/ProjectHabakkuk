using HBKPlatform.Models.DTO;

namespace HBKPlatform.Models.View.MyND
{
    public class ClientRecords
    {
        public string ClientName { get; set; }
        public int ClientId { get; set; }
        public List<ClientRecordLite> ClientRecordList { get; set; }
    }
}
