using HBKPlatform.Models.DTO;

namespace HBKPlatform.Repository;

public interface IRecordRepository
{
    public Task<ClientRecordDto> GetRecord(int recordId);
    public Task<List<ClientRecordLite>> GetClientRecordsLite(int clientId);
    //public Task UpdateRecord(ClientRecordDto recordDto);
    public Task<string> UpdateRecordBody(int recordId, string noteBody);
    public Task SetRecordPriority(int recordId, bool priority);
    public Task CreateRecord(ClientRecordDto recordDto);
}