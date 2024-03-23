using HBKPlatform.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HBKPlatform.Areas.Clinic.Controllers;

/// <summary>
/// HBKPlatform Clinic Reservation Controller.
/// Accept or reject practitioner reservations through this cont.
/// 
/// Author: Mark Brown
/// Authored: 20/03/2024
/// 
/// © 2024 NowDoctor Ltd.
/// </summary>
[Area("Clinic"), Authorize(Roles="ClinicManager")]
public class ReservationController(IRoomReservationService _roomRes): Controller
{
    public async Task <IActionResult> Index()
    {
        return View(await _roomRes.GetUpcomingReservationsClinic());
    }

    public async Task<IActionResult> ActionReservation(int reservationId, string action)
    {
        return Ok();
    }
    
    public async Task<IActionResult> CancelReservation(int reservationId)
    {
        return Ok();
    }
}
