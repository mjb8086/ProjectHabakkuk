using HBKPlatform.Models.DTO;
using HBKPlatform.Models.View;
using HBKPlatform.Repository;

namespace HBKPlatform.Services.Implementation;

/// <summary>
/// HBKPlatform Client Record service.
/// Middleware for client record controller and database functionality.
/// 
/// Author: Mark Brown
/// Authored: 09/01/2024
/// 
/// © 2024 NowDoctor Ltd.
/// </summary>
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
            ClientId = clientId,
            ClientRecords = await _recordRepo.GetClientRecordsLite(clientId)
        };
    }

    public async Task<ClientRecordDto> GetClientRecord(int? recordId, int? clientId)
    {
        if (recordId.HasValue)
        {
            var clientRecord = await _recordRepo.GetRecord(recordId.Value);
            clientRecord.ClientName = _cache.GetClientName(clientRecord.ClientId);
            return clientRecord;
        }
        if (clientId.HasValue)// is create mode, return blank model.
        {
            return new ClientRecordDto()
            {
                ClientId = clientId.Value, 
                ClientName = _cache.GetClientName(clientId.Value)
            };
        }

        throw new InvalidOperationException("Missing data");
    }

    public async Task CreateRecord(ClientRecordDto recordDto)
    {
        recordDto.ClinicId = _userService.GetClaimFromCookie("ClinicId");
        await _recordRepo.CreateRecord(recordDto);
    }
}