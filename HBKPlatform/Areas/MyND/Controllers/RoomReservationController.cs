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
    public class RoomReservationController(IRoomService _roomService): Controller
    {
        public async Task<IActionResult> MyReservations()
        {
            return View();
        }
        
        public async Task<IActionResult> MakeAReservation()
        {
            return View(await _roomService.GetRoomsForBooking());
        }

        public async Task<IActionResult> ChooseDateTime(int roomId)
        {
            return Ok();
        }

        public async Task<IActionResult> ConfirmReservation(int roomId, int weekNum, int timeslotId)
        {
            return Ok();
        }

        public async Task<IActionResult> DoMakeReservation(int roomId, int weekNum, int timeslotId)
        {
            return Ok();
        }
    }
}