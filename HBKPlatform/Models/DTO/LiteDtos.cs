using HBKPlatform.Globals;

namespace HBKPlatform.Models.DTO
{
    public struct ClientDetailsLite
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public int PracticeId { get; set; }
    }

    public class PractitionerDetailsLite
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public int PracticeId { get; set; }
    }

    public class PractitionerDetailsUac : PractitionerDetailsLite
    {
        public bool HasLockout { get; set; } 
        public DateTimeOffset? LockoutEnd { get; set; }
    }

    public struct ClientRecordLite
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime Date { get; set; }
        public Enums.RecordVisibility Visibility { get; set; }
        public bool IsPriority { get; set; }
    }

    public struct TreatmentLite
    {
        public int Id { get; set; }
        public Enums.TreatmentRequestability Requestability { get; set; }
        public double Cost { get; set; }
        public string Title { get; set; }
    }

    public struct TimeslotLite
    {
        public int Id { get; set; }
        public int WeekNum { get; set; }
        public string Description { get; set; }
    }

    public class PractitionerBookingFormModel
    {
        public string TimeslotWeekNum { get; set; }
        public int TreatmentId { get; set; }
        public int ClientId { get; set; }
        public int RoomResId { get; set; }

        public int[] ParseTsWeekNum()
        {
            return TimeslotWeekNum.Split('|').Select(int.Parse).ToArray();
        }
    }

    public class CancelAppointmentFormModel
    {
        public string Reason { get; set; }
    }

    public class RoomLite
    {
        public int Id { get; set; }
        public string Title { get; set; }
    }

    public class RoomReservationLite
    {
        public string RoomTitle { get; set; }
        public string When { get; set; }
        public string Whom { get; set; }
        public int Id { get; set; }
        public Enums.ReservationStatus Status { get; set; }
    }

    public class ClinicLite
    {
        public string Name { get; set; }
        public int Id { get; set; }
    }
    
}
