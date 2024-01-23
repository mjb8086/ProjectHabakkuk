using HBKPlatform.Models.DTO;

namespace HBKPlatform.Models.View;

public struct MyNDBookClientTreatment
{
    public List<ClientDetailsLite> Clients { get; set; }
    public List<TreatmentLite> Treatments { get; set; }
    public List<TimeslotLite> Timeslots { get; set; }
}