using HBKPlatform.Models.DTO;
using HBKPlatform.Repository;
using HBKPlatform.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HBKPlatform.Areas.MyND.Controllers
{
    /// <summary>
    /// HBKPlatform MyND Record Controller.
    /// All views and API routes for Practitioner's client records.
    /// 
    /// Author: Mark Brown
    /// Authored: 03/01/2024
    /// 
    /// © 2024 NowDoctor Ltd.
    /// </summary>
    [Area("MyND"), Authorize(Roles="Practitioner")]
    public class RecordController(IClientRecordService _recordService, IRecordRepository _recordRepo): Controller
    {
        /// <summary>
        /// Get a list of clients to read their records
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View(await _recordService.GetClientRecordsIndex());
        }
    
        /// <summary>
        /// Get all records summarised for the client
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> ClientRecords(int clientId)
        {
            return View(await _recordService.GetClientRecords(clientId));
        }
    
        /// <summary>
        /// Get the client's record, denoted by record ID
        /// Check that the record belongs to the client.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> ClientRecord(int? recordId, int? clientId)
        {
            return View(await _recordService.GetClientRecord(recordId : recordId, clientId : clientId));
        }

        [HttpPut]
        public async Task<IActionResult> UpdateRecord([FromBody] UpdateRecordLite record)
        {
            if (!ModelState.IsValid) throw new Exception("Model bad");
            return Ok(await _recordService.UpdateRecord(record));
        }

        [HttpPost]
        public async Task<IActionResult> SetRecordPriority(int recordId, bool isPriority)
        {
            await _recordRepo.SetRecordPriority(recordId, isPriority);
            return RedirectToRoute(new { controller = "Record", action = "ClientRecord", recordId=recordId });
        }

        [HttpPost]
        public async Task<IActionResult> CreateRecord([FromBody] ClientRecordDto recordDto)
        {
            if (!ModelState.IsValid) throw new Exception("Model bad");
            return Ok(await _recordService.CreateRecord(recordDto));
        }
    
        [HttpGet]
        public async Task<IActionResult> Delete(int clientId, int recordId)
        {
            // todo: Verify user has right to delete in record service
            await _recordRepo.DeleteRecord(recordId);
            return RedirectToRoute(new {controller = "Record", action = "ClientRecords", clientId = clientId});
        }
    
    }
}