using HBKPlatform.Globals;
using HBKPlatform.Models.DTO;

namespace HBKPlatform.Repository
{
    public interface IAppointmentRepository
    {
        public Task CreateAppointment(AppointmentDto appointmentDto);
        public Task<AppointmentDto> GetAppointment(int appointmentId);
        public Task<List<AppointmentDto>> GetAppointmentsForClient(int clientId);
        public Task<List<AppointmentDto>> GetAppointmentsForPractitioner(int pracId);
        public Task<List<AppointmentDto>> GetFutureAppointmentsForPractitioner(int pracId, DateTime now);
        public Task CancelAppointment(int appointmentId, string reason, Enums.AppointmentStatus actioner);
        public Task<bool> CheckForDoubleBookingsAnyTenant(int weekNum, int timeslotId, int roomId, int? currentRoomResId = null);
        public Task<List<TimeslotDto>> GetFutureOccupiedTimeslotsForRoomAnyTenancy(int roomId, DateTime now);

    }
}