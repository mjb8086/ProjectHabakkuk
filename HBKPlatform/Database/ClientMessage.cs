using System.ComponentModel.DataAnnotations;
using HBKPlatform.Globals;

namespace HBKPlatform.Database
{
     public class ClientMessage : HbkBaseEntity
     {
          // Rows
          public int? PreviousMessageId { get; set; }
          public int PractitionerId { get; set; }
          public int ClientId { get; set; }

          [DataType(DataType.MultilineText)]
          public string MessageBody { get; set; }
          public DateTime? DateOpened { get; set; }
          public Enums.MessageStatus MessageStatusPractitioner { get; set; }
          public Enums.MessageStatus MessageStatusClient { get; set; }
          public Enums.MessageOrigin MessageOrigin { get; set; }

          // EF Navigations
          public ClientMessage PreviousMessage { get; set; }
          private Practice Practice { get; set; }
          private Practitioner Practitioner { get; set; }
          private Client Client { get; set; }
     }
}
