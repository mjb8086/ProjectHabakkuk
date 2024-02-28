using HBKPlatform.Globals;

namespace HBKPlatform.Models.DTO
{
    public class TreatmentDto
    {
        public int Id { get; set; }
        public int ClinicId { get; set; }
        public Enums.TreatmentRequestability Requestability { get; set; }
        public double Cost { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string? Img { get; set; }
    }
}