using HBKPlatform.Globals;
using HBKPlatform.Models.DTO;
using HBKPlatform.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HBKPlatform.Areas.Client.Controllers
{
    /// <summary>
    /// HBKPlatform Client appointment controller.
    /// 
    /// Author: Mark Brown
    /// Authored: 12/01/2024
    /// 
    /// © 2024 NowDoctor Ltd.
    /// </summary>
    [Area("Client"), Authorize(Roles="Client")]
    public class AppointmentController(IBookingService _bookingService, ITreatmentService _treatmentService, IConfigurationService _config) : Controller
    {
        public async Task<IActionResult> Index()
        {
            return View(await _bookingService.GetClientUpcomingAppointmentsView());
        }
    
        [Route("booking/")]
        public async Task<IActionResult> Booking()
        {
            if (await _config.IsSettingEnabled("SelfBookingEnabled"))
            {
                return View("Booking/Index", await _treatmentService.GetTreatmentsViewForClient());
            }
            return RedirectToRoute(new { controller = "Appointment", action = "Index" });
        }
    
        [Route("booking/timeslotselect")]
        public async Task<IActionResult> TimeslotSelect(int treatmentId)
        {
            if (await _config.IsSettingEnabled("SelfBookingEnabled"))
            {
                return View("Booking/TimeslotSelect", await _bookingService.GetAvailableTimeslotsClientView(treatmentId));
            }
            return RedirectToRoute(new { controller = "Appointment", action = "Index" });
        }
    
        [Route("booking/bookingconfirm")]
        public async Task<IActionResult> BookingConfirm(int treatmentId,  int timeslotId, int weekNum)
        {
            if (await _config.IsSettingEnabled("SelfBookingEnabled"))
            {
                return View("Booking/BookingConfirm", await _bookingService.GetBookingConfirmModel(treatmentId, timeslotId, weekNum));
            }
            return RedirectToRoute(new { controller = "Appointment", action = "Index" });
        }
    
        [Route("booking/bookingconfirmed")]
        public async Task<IActionResult> BookingConfirmed(int treatmentId, int timeslotId, int weekNum)
        {
            if (await _config.IsSettingEnabled("SelfBookingEnabled"))
            {
                var model = await _bookingService.DoBookingClient(treatmentId, timeslotId, weekNum);
                return View("Booking/BookingConfirmed", model);
            }
            return RedirectToRoute(new { controller = "Appointment", action = "Index" });
        }
    
        [Route("booking/cancel")]
        public async Task<IActionResult> CancelBooking(int appointmentId)
        {
            return View("Booking/CancelBooking", await _bookingService.GetBookingCancelView(appointmentId));
        }

        [Route("booking/docancel")]
        public async Task<IActionResult> DoCancelBooking([FromQuery] int appointmentId, [FromForm] CancelAppointmentFormModel model)
        {
            if (!ModelState.IsValid) throw new MissingFieldException("Model bad");
            await _bookingService.DoCancelBooking(appointmentId, model.Reason, Enums.AppointmentStatus.CancelledByClient);
            TempData["Message"] = "Successfully cancelled booking";
            return RedirectToRoute(new { controller = "Appointment", action = "Index" });
        }
    }
}
