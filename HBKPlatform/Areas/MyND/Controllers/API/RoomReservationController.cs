using HBKPlatform.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HBKPlatform.Areas.MyND.Controllers.API;

/// <summary>
/// HBKPlatform Room Reservation API controller.
/// 
/// Author: Mark Brown
/// Authored: 22/05/2024
/// 
/// © 2024 NowDoctor Ltd.
/// </summary>
[Area("MyND"), Authorize(Roles="Practitioner"), Route("/api/mynd/roomreservation/[action]")]
public class RoomReservationController(IRoomReservationService _roomResService): Controller
{
    public async Task<IActionResult> GetReservationsLite()
    {
        return Ok(await _roomResService.GetHeldReservationsPractitioner());
    }
}