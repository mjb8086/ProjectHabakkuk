using HBKPlatform.Database;
using HBKPlatform.Globals;
using HBKPlatform.Models.DTO;
using HBKPlatform.Models.View.MCP;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace HBKPlatform.Repository.Implementation
{
    /// <summary>
    /// Clinic Repository.
    ///
    /// Author: Mark Brown
    /// Authored: 13/12/2023
    /// 
    /// © 2023 NowDoctor Ltd.
    /// </summary>
    public class ClinicRepository(ApplicationDbContext _db) : IClinicRepository
    {
    
    }
}