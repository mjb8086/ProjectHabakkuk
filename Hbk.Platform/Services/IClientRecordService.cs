using Hbk.Platform.Models.DTO;
using Hbk.Platform.Models.View.MyND;
using Hbk.Platform.Models.View;

namespace Hbk.Platform.Services
{
    public interface IClientRecordService
    {
        public Task<ClientRecordsIndex> GetClientRecordsIndex();
        public Task<ClientRecords> GetClientRecords(int clientId);
        public Task<List<ClientRecordLite>> GetPopulatedLiteRecords(bool isPriority);
        public Task<FullClientRecordDto> GetClientRecord(int? recordId, int? clientId);
        public Task <FullClientRecordDto> CreateRecord(ClientRecordDto recordDto);
        public Task<FullClientRecordDto> UpdateRecord(UpdateRecordLite recordDto);
    }
}