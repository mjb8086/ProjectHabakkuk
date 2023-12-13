using HBKPlatform.Globals;

namespace HBKPlatform.Models.DTO;

public class ClientMessageDto
{
    public string MessageBody { get; set; }
    public Enums.MessageStatus MessageStatus { get; set; }
    public Enums.MessageOrigin MessageOrigin { get; set; }
}