using Hbk.Platform.Models;
using Hbk.Platform.Models.View.MyND;

namespace Hbk.Platform.Services
{
    public interface IAvailabilityManagementService
    {
        public Task<AvailabilityManagementIndex> GetAvailabilityManagementIndexModel(int? roomId = null);

        public Task UpdatePractitionerForWeek(int weekNum, UpdatedAvailability model);
        public Task<AvailabilityModel> GetPractitionerModelForWeek(int weekNum);
        public Task<AvailabilityModel> GetPractitionerModelForIndef();
        public Task RevertPractitionerForWeek(int weekNum);
        public Task UpdatePractitionerForIndef(UpdatedAvailability model);
        public Task ClearPractitonerForIndef();

        
        public Task<AvailabilityModel> GetRoomModelForWeek(int roomId, int weekNum);
        public Task<AvailabilityModel> GetRoomModelForIndef(int roomId);

        public Task UpdateRoomForWeek(int roomId, int weekNum, UpdatedAvailability model);
        public Task ClearRoomForWeek(int roomId, int weekNum);
        public Task UpdateRoomForIndef(int roomId, UpdatedAvailability model);
        public Task ClearRoomForIndef(int roomId);
    }
}