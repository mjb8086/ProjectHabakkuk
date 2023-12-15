using System.ComponentModel.DataAnnotations;

namespace HBKPlatform.Database;

public class Client : HbkBaseEntity
{
    public Title Title { get; set; }
    public Sex Sex { get; set; }
    public string Forename { get; set; }
    public string Surname { get; set; }

    [DataType(DataType.Date)]
    public DateTime DateOfBirth { get; set; }
    public string? Img { get; set; }
    public string? UserId { get; set; }
    public string Telephone { get; set; }
    [DataType(DataType.MultilineText)]
    public string? Address { get; set; }

    public int ClinicId { get; set; }
    public Clinic Clinic { get; set; }

    public User User { get; set; }
    public virtual ICollection<ClientMessage> ClientMessages { get; set; }
}