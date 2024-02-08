/******************************
* HBK Practitioner Model
* Defines practitioners of all stripes and types.
*
* Author: Mark Brown
* Authored: 10/09/2022
******************************/

using System.ComponentModel.DataAnnotations;
using HBKPlatform.Globals;

namespace HBKPlatform.Database
{
    public class Practitioner : HbkBaseEntity
    {
        [Required]
        public string Forename { get; set; }
        [Required]
        public string Surname { get; set; }
        [Required]
        public Enums.Title Title { get; set; }
        public string? Location { get; set; }
        public string? Bio { get; set; }
        [DataType(DataType.Date)]
        public DateOnly DateOfBirth { get; set; }
        public string? Img { get; set; }
        public Enums.Sex Sex { get; set; }

        public int ClinicId { get; set; }
        public Clinic Clinic { get; set; }
        public string? UserId { get; set; }
        public User User { get; set; }
        
        public virtual ICollection<ClientMessage> ClientMessages { get; set; }
    }

}

