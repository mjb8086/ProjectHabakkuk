using System.ComponentModel.DataAnnotations;

namespace HBKPlatform.Database;

public class Client : HbkBaseEntity
{
    public Title Title { get; set; }
    public Sex Sex { get; set; }
    [DataType(DataType.Date)]
    public DateTime DateOfBirth { get; set; }
    public virtual Uri? Img { get; set; }
    public int UserId { get; set; }
    public string Telephone { get; set; }
    [DataType(DataType.MultilineText)]
    public string Address { get; set; }

    public int ClinicId { get; set; }
    public Clinic Clinic { get; set; }
    
    public virtual ICollection<ClientMessage> ClientMessages { get; set; }
}