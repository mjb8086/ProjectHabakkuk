using HBKPlatform.Models.DTO;
using HBKPlatform.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HBKPlatform.Areas.Clinic.Controllers;

/// <summary>
/// HBKPlatform Clinic Room Controller.
/// 
/// Author: Mark Brown
/// Authored: 20/03/2024
/// 
/// © 2024 NowDoctor Ltd.
/// </summary>
[Area("Clinic"), Authorize(Roles="ClinicManager")]
public class RoomController(IRoomService _roomService): Controller
{
    public async Task <IActionResult> Index()
    {
        return RedirectToRoute(new { area = "Clinic", controller = "Room", action = "List" });
    }
    
    public async Task <IActionResult> Availability()
    {
        return View();
    }
    
    public async Task <IActionResult> List()
    {
        return View(await _roomService.GetClinicRooms());
    }
    
    public async Task <IActionResult> AddEdit(int? roomId)
    {
        return roomId.HasValue ? View(await _roomService.GetRoom(roomId.Value)) : View(new RoomDto());
    }

    [HttpPost]
    public async Task<IActionResult> DoCreateRoom([FromForm] RoomDto room)
    {
        if (!ModelState.IsValid) throw new MissingFieldException("Model bad");
        await _roomService.Create(room);
        TempData["Message"] = "Successfully created a new room.";
        return RedirectToRoute(new { area = "Clinic", controller = "Room", action = "List" });
    }
    
    [HttpPost] // Cannot do PUT until we use JSON over an API
    public async Task<IActionResult> DoUpdateRoom(int roomId, [FromForm] RoomDto room)
    {
        room.Id = roomId;
        if (!ModelState.IsValid) throw new MissingFieldException("Model bad");
        await _roomService.Update(room);
        TempData["Message"] = "Room data updated.";
        return RedirectToRoute(new { area = "Clinic", controller = "Room", action = "List" });
    }
}
