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
          ClientAndPrac = 2, PracOnly = 1, None = 0
     }

     public enum Day
     {
          Monday, Tuesday, Wednesday, Thursday, Friday, Saturday, Sunday
     }

     public enum TimeslotAvailability
     {
          Unavailable, Available
     }

     public enum AppointmentStatus
     {
          Approved, Requested, CancelledByClient, CancelledByPractitioner
     }
     
}