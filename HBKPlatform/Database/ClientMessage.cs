using System.ComponentModel.DataAnnotations;

namespace HBKPlatform.Database;

public class ClientMessage : HbkBaseEntity
{
     // Rows
     public int PreviousMessageId { get; set; }
     public int ClientId { get; set; }
     public int PractitionerId { get; set; }
     [DataType(DataType.MultilineText)]
     public string MessageBody { get; set; }
     public DateTime? DateOpened { get; set; }
     public MessageStatus MessageStatus { get; set; }
     public MessageOrigin MessageOrigin { get; set; }

     // EF Navigations
     public ClientMessage PreviousMessage { get; set; }
     public Client Client { get; set; }
     public Practitioner Practitioner { get; set; }
}

public enum MessageStatus
{
     Unread,
     Read,
     Deleted,
     Archived
}

public enum MessageOrigin
{
     Client,
     Practitioner
}