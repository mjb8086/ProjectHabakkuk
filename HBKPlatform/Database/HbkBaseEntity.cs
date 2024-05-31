using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HBKPlatform.Database
{
    public abstract class HbkBaseEntity
    {
        // inheritance and sequencing bug needs fixed with pgsql driver (not my problem)
        // meaning *everything* in the db that uses this col will share the same sequence
        [Key, Column(Order = 1)]
        public int Id { get; set; } 
        public int TenancyId { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime DateCreated { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime? DateModified { get; set; }
        public string? CreateActioner { get; set; }
        public string? ModifyActioner { get; set; }
        public virtual Tenancy Tenancy { get; set; }
    }
}

