using HBKPlatform.Models.DTO;

namespace HBKPlatform.Repository
{
    public interface IRecordRepository
    {
        public Task<FullClientRecordDto> GetRecord(int recordId);
        public Task<List<ClientRecordLite>> GetClientRecordsLite(int clientId, bool priorityOnly = false);
        public Task<FullClientRecordDto> UpdateRecord(FullClientRecordDto recordDto);
        public Task<FullClientRecordDto> UpdateRecordLite(UpdateRecordLite recordDto);
        public Task SetRecordPriority(int recordId, bool priority);
        public Task<FullClientRecordDto> CreateRecord(ClientRecordDto recordDto);
        public Task DeleteRecord(int recordId);
    }
}