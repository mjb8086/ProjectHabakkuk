using HBKPlatform.Models.API.MyND;

namespace HBKPlatform.Services;

public interface IReceptionService
{
    public Task<ReceptionSummaryData> GetReceptionSummaryData();
}