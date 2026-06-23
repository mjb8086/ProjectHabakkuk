using Hbk.Models.DTO;

namespace Hbk.Models.View.MyND
{
    public class ClientRecords
    {
        public string ClientName { get; set; }
        public int ClientId { get; set; }
        public List<ClientRecordLite> ClientRecordList { get; set; }
    }
}
