using HBKPlatform.Database;
using HBKPlatform.Globals;
using HBKPlatform.Models.DTO;

namespace HBKPlatform.Repository
{
    public interface IAppointmentRepository
    {
        public Task CreateAppointment(AppointmentDto appointmentDto);
        public Task<AppointmentDto> GetAppointment(int appointmentId);
        public Task<List<AppointmentDto>> GetAppointmentsForClient(int clientId);
        public Task<List<AppointmentDto>> GetAppointmentsForPractitioner(int pracId, Enums.AppointmentStatus? status = null);

        public Task<List<AppointmentDto>> GetFutureAppointmentsForPractitioner(int pracId, int currentWeekNum,
            int currentTick, bool liveOnly, int? limit = null);
//        public Task<int> GetFutureAppointmentCountForPractitioner(int pracId, DateTime now, string dbStartDate, bool liveOnly = true);
        public Task CancelAppointment(int appointmentId, string reason, Enums.AppointmentStatus actioner);

        public Task<bool> CheckForDoubleBookingsAnyTenant(AppointmentRequestDto appointmentReq, int roomId,
            int? currentRoomResId = null);
        public Task<List<TimeslotDto>> GetFutureOccupiedTimeslotsForRoomAnyTenancy(int roomId, DateTime now);
        
        // Statistical methods
        public Task<int> GetNumberOfCompletedAppointments(int weekNum, int currentTick, int pracId);
        
        // NEW TS METHODS
        public Task<bool> CheckForDoubleBooking(int practitionerId, int weekNum, int startTick, int endTick);
        public Task<List<TimeblockDto>> GetFutureAppointmentTimeblocks(int pracId, int startWeek, int endWeek, int currentTick);

    }
}