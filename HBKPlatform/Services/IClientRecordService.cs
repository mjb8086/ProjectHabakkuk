using HBKPlatform.Models.DTO;
using HBKPlatform.Models.View;

namespace HBKPlatform.Services;

public interface IClientRecordService
{
    public Task<MyNDClientRecordsIndex> GetClientRecordsIndex();
    public Task<MyNDClientRecords> GetClientRecords(int clientId);
    public Task<ClientRecordDto> GetClientRecord(int recordId);
}