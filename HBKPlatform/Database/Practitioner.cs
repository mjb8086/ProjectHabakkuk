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
        public string Location { get; set; }
        public string Bio { get; set; }
        [DataType(DataType.Date)]
        public DateTime DOB { get; set; }
        public string? Img { get; set; }

//        public virtual ICollection<Review> Reviews { get; set; }
    }

    public enum Title
    {
        Dr, Mr, Mrs, Miss, Ms, Lord, Cpl, Pope
    }
}

