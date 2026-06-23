using Hbk.Platform.Globals;
using Hbk.Platform.Models.DTO;

namespace Hbk.Platform.Models
{
    public class ClientMessageConversationModel
    {
        public string Recipient { get; set; }
        public string Sender { get; set; }
        public int ClientId { get; set; }
        public int PractitionerId { get; set; }
        public DateTime MostRecentlyReceived { get; set; }
        public Enums.MessageOrigin CurrentConverser { get; set; }
        public List<ClientMessageDto> Messages { get; set; }
    }
}