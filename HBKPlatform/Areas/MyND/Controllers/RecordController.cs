using Microsoft.AspNetCore.Mvc;

namespace HBKPlatform.Controllers;

/// <summary>
/// HBKPlatform MyND Record Controller.
/// All views and API routes for Practitioner's client records.
/// 
/// Author: Mark Brown
/// Authored: 03/01/2024
/// 
/// © 2024 NowDoctor Ltd.
/// </summary>
[Area("MyND")]
public class RecordController: Controller
{
    /// <summary>
    /// Get a list of clients to read their records
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        return View();
    }
    
    /// <summary>
    /// Get all records summarised for the client
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Client()
    {
        return View();
    }
    
    /// <summary>
    /// Get the client's record, denoted by record ID
    /// Check that the record belongs to the client.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> ClientRecord()
    {
        return View();
    }
}