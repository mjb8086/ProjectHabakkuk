using System.ComponentModel.DataAnnotations;
using HBKPlatform.Globals;

namespace HBKPlatform.Database;

public class ClientMessage : HbkBaseEntity
{
     // Rows
     public int? PreviousMessageId { get; set; }
     public string SenderId { get; set; }
     public string RecipientId { get; set; }
     public int ClinicId { get; set; }

     [DataType(DataType.MultilineText)]
     public string MessageBody { get; set; }
     public DateTime? DateOpened { get; set; }
     public Enums.MessageStatus MessageStatus { get; set; }

     // EF Navigations
     public ClientMessage PreviousMessage { get; set; }
     public User Sender { get; set; }
     public User Recipient { get; set; }
     private Clinic Clinic { get; set; }
}
