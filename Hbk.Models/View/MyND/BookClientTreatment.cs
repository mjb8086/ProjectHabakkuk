using Hbk.Models.DTO;

namespace Hbk.Models.View.MyND
{
    public struct BookClientTreatment
    {
        public List<ClientDetailsLite> Clients { get; set; }
        public List<TreatmentLite> Treatments { get; set; }
        public List<TimeslotLite> Timeslots { get; set; }
        public List<RoomReservationLite> HeldReservations { get; set; }
    }
}