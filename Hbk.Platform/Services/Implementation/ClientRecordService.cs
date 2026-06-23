using Hbk.Common.Helpers;
using Hbk.Models.DTO;
using Hbk.Models.View.MyND;
using Hbk.Platform.Helpers;
using Hbk.Platform.Repository;

namespace Hbk.Platform.Services.Implementation
{
    /// <summary>
    /// Hbk.Platform Client Record service.
    /// Middleware for client record controller and database functionality.
    /// 
    /// Author: Mark Brown
    /// Authored: 09/01/2024
    /// 
    /// © 2024 NowDoctor Ltd.
    /// </summary>
    public class ClientRecordService(IUserService _userService, ICacheService _cache, IRecordRepository _recordRepo): IClientRecordService
    {
        public async Task<ClientRecordsIndex> GetClientRecordsIndex()
        {
            return new ClientRecordsIndex
            {
                ClientDetails = await _cache.GetPracticeClientDetailsLite()
            };
        }
        public async Task<ClientRecords> GetClientRecords(int clientId)
        {
            return new ClientRecords
            {
                ClientName = _cache.GetClientName(clientId),
                ClientId = clientId,
                ClientRecordList = await _recordRepo.GetClientRecordsLite(clientId)
            };
        }

        public async Task<List<ClientRecordLite>> GetPopulatedLiteRecords(bool isPriority)
        {
            var liteRecords = await _recordRepo.GetRecordsLite(isPriority);
            foreach (var record in liteRecords)
            {
                record.ClientName = _cache.GetClientName(record.ClientId);
                record.DisplayDate = DateTimeHelper.GetFriendlyDateTimeString(record.Date);
            }
            return liteRecords;
        }

        public async Task<FullClientRecordDto> GetClientRecord(int? recordId, int? clientId)
        {
            if (recordId.HasValue)
            {
                var clientRecord = await _recordRepo.GetRecord(recordId.Value);
                clientRecord.ClientName = _cache.GetClientName(clientRecord.ClientId);
                return clientRecord;
            }
            if (clientId.HasValue)// is create mode, return blank model.
            {
                return new FullClientRecordDto()
                {
                    ClientId = clientId.Value, 
                    ClientName = _cache.GetClientName(clientId.Value)
                };
            }

            throw new MissingFieldException("Record Id or client ID missing.");
        }

        public async Task<FullClientRecordDto> CreateRecord(ClientRecordDto recordDto)
        {
            recordDto.PractitionerId = _userService.GetClaimFromCookie("PractitionerId");
            return await _recordRepo.CreateRecord(recordDto);
        }

        public async Task<FullClientRecordDto> UpdateRecord(UpdateRecordLite recordDto)
        {
            return await _recordRepo.UpdateRecordLite(recordDto);
        }
    }
}
