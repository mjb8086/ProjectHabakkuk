using Hbk.Platform.Models.API.MyND;

namespace Hbk.Platform.Services;

public interface IReceptionService
{
    public Task<ReceptionSummaryData> GetReceptionSummaryData();
}