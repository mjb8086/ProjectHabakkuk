using Hbk.Models.API.API.MyND;
using Hbk.Models.View.MyND;

namespace Hbk.Platform.Services;

public interface IReceptionService
{
    public Task<ReceptionSummaryData> GetReceptionSummaryData();
    public Task<ReceptionModel> GetReceptionModel();
}
