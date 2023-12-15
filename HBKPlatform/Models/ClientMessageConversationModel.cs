using HBKPlatform.Models.DTO;

namespace HBKPlatform.Models;

public class ClientMessageConversationModel
{
    public string Recipient { get; set; }
    public int ClientId { get; set; }
    public int PractitionerId { get; set; }
    public DateTime MostRecentlyReceived { get; set; }
    public List<ClientMessageDto> Messages { get; set; }
}