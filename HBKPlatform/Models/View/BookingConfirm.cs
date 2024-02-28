namespace HBKPlatform.Models.View
{
    public struct BookingConfirm
    {
        public int WeekNum { get; set; }
        public int TreatmentId { get; set; }
        public int TimeslotId { get; set; }
        public int? ClientId { get; set; }
        public string TreatmentTitle { get; set; }
        public string PracctitionerName { get; set; }
        public string ClientName { get; set; }
        public string BookingDate { get; set; }
    }
}