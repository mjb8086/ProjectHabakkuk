using Hbk.Platform.Services;
using Hbk.Platform.Exceptions;
using Hbk.Platform.Globals;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hbk.Platform.Areas.Clinic.Controllers;

/// <summary>
/// Hbk.Platform Clinic Reservation Controller.
/// Accept or reject practitioner reservations through this controller.
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

    public async Task<IActionResult> ApproveReservation(int reservationId)
    {
        await _roomRes.ApproveReservation(reservationId);
        TempData["Message"] = $"Reservation request has been successfully approved.";
        return RedirectToRoute(new { area = "Clinic", controller = "Reservation", action = "Index" });
    }
    
    public async Task<IActionResult> DenyReservation(int reservationId)
    {
        await _roomRes.DenyReservation(reservationId);
        TempData["Message"] = $"Reservation request has been denied.";
        return RedirectToRoute(new { area = "Clinic", controller = "Reservation", action = "Index" });
    }
    
    public async Task<IActionResult> CancelReservation(int reservationId)
    {
        await _roomRes.CancelAsClinic(reservationId);
        TempData["Message"] = $"Reservation has been cancelled.";
        return RedirectToRoute(new { area = "Clinic", controller = "Reservation", action = "Index" });
    }
}
