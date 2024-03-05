using HBKPlatform.Models.DTO;
using HBKPlatform.Models.View;
using HBKPlatform.Models.View.MyND;
using HBKPlatform.Repository;

namespace HBKPlatform.Services.Implementation
{
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
        public async Task<ClientRecordsIndex> GetClientRecordsIndex()
        {
            return new ClientRecordsIndex
            {
                ClientDetails = await _cache.GetClinicClientDetailsLite()
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
            recordDto.ClinicId = _userService.GetClaimFromCookie("ClinicId");
            recordDto.PractitionerId = _userService.GetClaimFromCookie("PractitionerId");
            return await _recordRepo.CreateRecord(recordDto);
        }

        public async Task<FullClientRecordDto> UpdateRecord(UpdateRecordLite recordDto)
        {
            return await _recordRepo.UpdateRecordLite(recordDto);
        }
    }
}
