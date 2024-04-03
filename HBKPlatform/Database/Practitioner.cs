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
        public string? ClientBio { get; set; }
        public string? ClinicBio { get; set; }
        public string? GmcNumber { get; set; }
        public string? Credentials { get; set; }
        [DataType(DataType.Date)]
        public DateOnly DateOfBirth { get; set; }
        public string? Img { get; set; }
        public Enums.Sex Sex { get; set; }

        public int PracticeId { get; set; }
        public Practice Practice { get; set; }
        public string? UserId { get; set; }
        public User User { get; set; }
        
        public virtual ICollection<ClientMessage> ClientMessages { get; set; }
        public virtual ICollection<ClientPractitioner> ClientPractitioners { get; set; }
    }

}

