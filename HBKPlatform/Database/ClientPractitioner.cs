using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace HBKPlatform.Database
{
    /// <summary>
    /// HBKPlatform ClientPractitioner entity.
    /// Join table for client-practitioner access permissions.
    /// 
    /// Author: Mark Brown
    /// Authored: 26/02/2024
    /// 
    /// © 2024 NowDoctor Ltd.
    /// </summary>
    /// 
    [PrimaryKey("ClientId", "PractitionerId")]
    public class ClientPractitioner 
    {
        public int ClientId { get; set; }
        public int PractitionerId { get; set; }
        public int TenancyId { get; set; }
    
        // EF Navigations
        public virtual Client Client { get; set; }
        public virtual Practitioner Practitioner { get; set; }
        public virtual Tenancy Tenancy { get; set; }
    }
}