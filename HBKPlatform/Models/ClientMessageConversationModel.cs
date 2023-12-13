using HBKPlatform.Models.DTO;

namespace HBKPlatform.Models;

public class ClientMessageConversationModel
{
    public string Recipient { get; set; }
    public string Sender { get; set; }
    public DateTime MostRecentlyReceived { get; set; }
    public List<ClientMessageDto> Messages { get; set; }
}