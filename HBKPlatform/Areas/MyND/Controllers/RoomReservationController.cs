using HBKPlatform.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HBKPlatform.Areas.MyND.Controllers
{
    /// <summary>
    /// HBKPlatform MyND Room Reservation Controller.
    /// Allow practitioner to search available clinic rooms and request reservations.
    /// 
    /// Author: Mark Brown
    /// Authored: 22/03/24
    /// 
    /// © 2024 NowDoctor Ltd.
    /// </summary>
    [Area("MyND"), Authorize(Roles="Practitioner")]
    public class RoomReservationController(IRoomService _roomService, IRoomReservationService _roomRes): Controller
    {
        public async Task<IActionResult> MyReservations()
        {
            return View(await _roomRes.GetUpcomingReservationsPractitioner());
        }
        
        public async Task<IActionResult> MakeAReservation()
        {
            return View(await _roomService.GetRoomsForBooking());
        }

        public async Task<IActionResult> TimeslotSelect(int roomId)
        {
            return View(await _roomRes.GetTimeslotSelectView(roomId));
        }

        public async Task<IActionResult> ConfirmReservation(int roomId, int weekNum, int timeslotId)
        {
            return View(await _roomRes.GetConfirmReservationView(roomId, weekNum, timeslotId));
        }

        public async Task<IActionResult> DoMakeReservation(int roomId, int weekNum, int timeslotId)
        {
            await _roomRes.Create(roomId, weekNum, timeslotId);
            TempData["Message"] = "Reservation request created. Please wait for the clinic's approval.";
            return RedirectToRoute(new { area = "MyNd", controller = "RoomReservation", action = "MyReservations" });
        }

        public async Task<IActionResult> CancelReservation(int reservationId)
        {
            await _roomRes.CancelAsPractitioner(reservationId);
            TempData["Message"] = "Reservation request cancelled.";
            return RedirectToRoute(new { area = "MyNd", controller = "RoomReservation", action = "MyReservations" });
        }
    }
}