using HBKPlatform.Models.DTO;
using HBKPlatform.Models.View;
using HBKPlatform.Repository;

namespace HBKPlatform.Services.Implementation;

public class ClientRecordService(IUserService _userService, ICacheService _cache, IRecordRepository _recordRepo): IClientRecordService
{
    public async Task<MyNDClientRecordsIndex> GetClientRecordsIndex()
    {
        return new MyNDClientRecordsIndex
        {
            ClientDetails = await _cache.GetClinicClientDetailsLite(_userService.GetClaimFromCookie("ClinicId"))
        };
    }
    public async Task<MyNDClientRecords> GetClientRecords(int clientId)
    {
        return new MyNDClientRecords
        {
            ClientName = _cache.GetClientName(clientId),
            ClientRecords = await _recordRepo.GetClientRecordsLite(clientId)
        };
    }

    public async Task<ClientRecordDto> GetClientRecord(int recordId)
    {
        var clientRecord = await _recordRepo.GetRecord(recordId);
        clientRecord.ClientName = _cache.GetClientName(clientRecord.ClientId);
        return clientRecord;
    }
}