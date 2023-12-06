/******************************
* HBK Practitioner Model
* Defines practitioners of all stripes and types.
*
* Author: Mark Brown
* Authored: 10/09/2022
******************************/

using System.ComponentModel.DataAnnotations;

namespace HBKPlatform.Database
{
    public class Practitioner : HbkBaseEntity
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public Title Title { get; set; }
        public string? Location { get; set; }
        public string? Bio { get; set; }
        [DataType(DataType.Date)]
        public DateTime DOB { get; set; }
        public virtual Uri? Img { get; set; }
        public Sex Sex { get; set; }

        public int ClinicId { get; set; }
        public virtual Clinic Clinic { get; set; }
    }

    public enum Title
    {
        Dr, Mr, Mrs, Miss, Ms, Lord, Cpl, Pope
    }

    public enum Sex
    {
        Male, Female, Other, NotSaying
    }
}

