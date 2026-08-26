using Hbk.Common.Globals;
using Hbk.Models.DTO;
using Hbk.Platform.Repository;
using Hbk.Platform.Services;
using Hbk.Platform.Services.Implementation;
using Moq;

namespace Hbk.Test;

public class ReceptionServiceUnitTests
{
    private const int PractitionerId = 42;
    private const string DbStartDate = "2024-01-01";

    [Fact]
    public async Task ReceptionDataIsLimitedAndMappedForTheDashboard()
    {
        var appointments = Enumerable.Range(1, 7)
            .Select(i => BuildAppointment($"Booking {i}", Enums.AppointmentStatus.Live, i))
            .Concat(Enumerable.Range(1, 8)
                .Select(i => BuildAppointment($"Cancellation {i}",
                    i % 2 == 0
                        ? Enums.AppointmentStatus.CancelledByClient
                        : Enums.AppointmentStatus.CancelledByPractitioner,
                    i + 10)))
            .ToList();
        var priorityItems = Enumerable.Range(1, 7)
            .Select(i => new ClientRecordLite
            {
                Id = i,
                ClientId = i,
                ClientName = $"Client {i}",
                Title = $"Priority {i}",
                Date = new DateTime(2026, 1, i),
                DisplayDate = $"{i} January 2026",
                IsPriority = true
            })
            .ToList();

        var bookingService = new Mock<IBookingService>();
        bookingService
            .Setup(x => x.GetUpcomingAppointmentsForPractitioner(PractitionerId, false, null))
            .ReturnsAsync(appointments);

        var userService = new Mock<IUserService>();
        userService.Setup(x => x.GetClaimFromCookie("PractitionerId")).Returns(PractitionerId);

        var configurationService = new Mock<IConfigurationService>();
        configurationService
            .Setup(x => x.GetSettingOrDefault("DbStartDate"))
            .ReturnsAsync(new SettingDto { Value = DbStartDate });

        var appointmentRepository = new Mock<IAppointmentRepository>();
        appointmentRepository
            .Setup(x => x.GetNumberOfCompletedAppointments(PractitionerId, DbStartDate, It.IsAny<DateTime>()))
            .ReturnsAsync(27);

        var recordService = new Mock<IClientRecordService>();
        recordService.Setup(x => x.GetPopulatedLiteRecords(true)).ReturnsAsync(priorityItems);

        var clientRepository = new Mock<IClientRepository>();
        clientRepository.Setup(x => x.GetClientCount()).Returns(16);

        var roomReservationService = new Mock<IRoomReservationService>();
        roomReservationService.Setup(x => x.GetHeldReservationsPractitioner()).ReturnsAsync([]);

        var messagingService = new Mock<IClientMessagingService>();
        messagingService
            .Setup(x => x.GetUnreadMessageDetailsAsPractitioner(PractitionerId))
            .ReturnsAsync([
                new UnreadMessageDetailLite { ClientId = 1, Name = "Client 1", UnreadMessageCount = 2 },
                new UnreadMessageDetailLite { ClientId = 2, Name = "Client 2", UnreadMessageCount = 3 }
            ]);

        var service = new ReceptionService(
            bookingService.Object,
            userService.Object,
            configurationService.Object,
            appointmentRepository.Object,
            recordService.Object,
            clientRepository.Object,
            roomReservationService.Object,
            messagingService.Object);

        var model = await service.GetReceptionModel();
        var summary = await service.GetReceptionSummaryData();

        Assert.Equal(ReceptionService.ReceptionItemLimit, model.RecentBookings.Count);
        Assert.Equal(ReceptionService.ReceptionItemLimit, model.RecentCancellations.Count);
        Assert.Equal(ReceptionService.ReceptionItemLimit, model.PriorityItems.Count);
        Assert.Equal("Priority 7", model.PriorityItems[0].Title);
        Assert.Equal(5, model.NumUnreadMessages);
        Assert.Equal(16, model.NumClientsRegistered);
        Assert.Equal(27, model.NumAppointmentsCompleted);
        Assert.Equal(2, summary.AdditionalUpcoming);
        Assert.Equal(3, summary.AdditionalCancellations);
    }

    private static AppointmentLite BuildAppointment(string clientName, Enums.AppointmentStatus status, int weekNum)
    {
        return new AppointmentLite
        {
            ClientName = clientName,
            TreatmentTitle = "Initial consultation",
            DateTime = "2026-01-01T10:00:00",
            Status = status,
            WeekNum = weekNum
        };
    }
}
