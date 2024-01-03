namespace HBKPlatform.Globals;

public static class Enums
{
     public enum MessageStatus
     {
          Unread,
          Read,
          Deleted,
          Archived
     }

     public enum MessageOrigin
     {
          Practitioner,
          Client
     }

     public enum RecordVisibility
     {
          ClientAndPrac, PracOnly, None
     }
     
     public enum TreatmentRequestability
     {
          ClientAndPrac, PracOnly, None
     }

     public enum Day
     {
          Monday, Tuesday, Wednesday, Thursday, Friday, Saturday, Sunday
     }

     public enum TimeslotAvailability
     {
          Unavailable, Available
     }
     
}