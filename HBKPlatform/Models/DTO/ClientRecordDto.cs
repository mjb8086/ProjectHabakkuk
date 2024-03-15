using HBKPlatform.Globals;

namespace HBKPlatform.Models.DTO
{
    // Use on create
    public class ClientRecordDto
    {
        public int ClientId { get; set; }
        public int PractitionerId { get; set; }
        public string? AppointmentDetails { get; set; }
        public DateTime? AppointmentDate { get; set; }
        public int? AppointmentId { get; set; }
        public Enums.RecordVisibility Visibility { get; set; }
        public string Title { get; set; }
        public string NoteBody { get; set; }
        public bool IsPriority { get; set; }
    }

// use when fetching
    public class FullClientRecordDto : ClientRecordDto
    {
        public int Id { get; set; }
        public string ClientName { get; set; }
        public string PractitionerName { get; set; }
    
        public DateTime? DateUpdated { get; set; }
        public DateTime DateCreated { get; set; }
    }

    public class UpdateRecordLite
    {
        public int Id { get; set; }
        public string NoteTitle { get; set; }
        public string NoteBody { get; set; }
    }
}