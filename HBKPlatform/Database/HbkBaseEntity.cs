using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HBKPlatform.Database
{
    public abstract class HbkBaseEntity
    {
        [Key]
        [Column(Order = 1)]
        public int Idx { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
    }
}

