using HBKPlatform.Globals;

namespace HBKPlatform.Models.DTO
{
    public class AppointmentDto
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public int PractitionerId { get; set; }
        public int TreatmentId { get; set; }
        public int WeekNum { get; set; }
        public int StartTick { get; set; }
        public int EndTick { get; set; }
        public int? RoomId { get; set; }
        public int? RoomReservationId { get; set; }
        public string? Note { get; set; }
        public string DateString { get; set; }
        public string TimeString { get; set; }
        public string ClientName { get; set; }
        public string PractitionerName { get; set; }
        public string TreatmentTitle { get; set; }
        public string? RoomDetails { get; set; }
        public Enums.AppointmentStatus Status { get; set; }

        public TimeslotDto Timeslot { get; set; }
    }

    public class AppointmentRequestDto
    {
        public int ClientId { get; set; }
        public int PractitionerId { get; set; }
        public int TreatmentId { get; set; }
        public int WeekNum { get; set; }
        public int StartTick { get; set; }
        public int EndTick { get; set; }
        public int? RoomId { get; set; }
        public int? RoomResId { get; set; }
        public string? Note { get; set; }
        
    }

}