using HBKPlatform.Models;
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
public class RoomController(IRoomService _roomService, IAvailabilityManagementService _availabilityMgmt): Controller
{
    public async Task <IActionResult> Index()
    {
        return RedirectToRoute(new { area = "Clinic", controller = "Room", action = "List" });
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
    
    //////////////////////////////////////////////////////////////////////////////// 
    // AVAILABILITY METHODS
    //////////////////////////////////////////////////////////////////////////////// 
    public async Task<IActionResult> AvailabilityManagement(int roomId)
    {
        if (roomId < 1) throw new MissingFieldException("RoomId required.");
        return View(await _availabilityMgmt.GetAvailabilityManagementIndexModel(roomId));
    }

    public async Task<IActionResult> SetWeekAvailability(int roomId, int weekNum)
    {
        return View(await _availabilityMgmt.GetRoomModelForWeek(roomId, weekNum));
    }

    public async Task<IActionResult> SetIndefAvailability(int roomId)
    {
        return View(await _availabilityMgmt.GetRoomModelForIndef(roomId));
    }

    [HttpPost]
    public async Task<IActionResult> DoSetAvailability(int roomId, int? weekNum, [FromBody] UpdatedAvailability model)
    {
        if (!ModelState.IsValid) throw new MissingFieldException("Missing data");
        if (weekNum.HasValue)
        {
            await _availabilityMgmt.UpdateRoomForWeek(roomId, weekNum.Value, model);
        }
        else
        {
            await _availabilityMgmt.UpdateRoomForIndef(roomId, model);
        }
        return Ok();
    }

    public async Task<IActionResult> DoRevertAvailability(int roomId, int? weekNum)
    {
        if (weekNum.HasValue)
        {
            await _availabilityMgmt.ClearRoomForWeek(roomId, weekNum.Value);
        }
        else
        {
            await _availabilityMgmt.ClearRoomForIndef(roomId);
        }

        return Ok();
    }
}
