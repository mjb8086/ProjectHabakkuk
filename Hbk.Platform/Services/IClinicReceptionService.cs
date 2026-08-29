using Hbk.Models.View.Clinic;

namespace Hbk.Platform.Services;

public interface IClinicReceptionService
{
    Task<ReceptionModel> GetReceptionModel();
}
