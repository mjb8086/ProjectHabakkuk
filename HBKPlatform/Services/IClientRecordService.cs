using HBKPlatform.Models.DTO;
using HBKPlatform.Models.View;
using HBKPlatform.Models.View.MyND;

namespace HBKPlatform.Services;

public interface IClientRecordService
{
    public Task<ClientRecordsIndex> GetClientRecordsIndex();
    public Task<ClientRecords> GetClientRecords(int clientId);
    public Task<ClientRecordDto> GetClientRecord(int? recordId, int? clientId);
    public Task CreateRecord(ClientRecordDto recordDto);
}