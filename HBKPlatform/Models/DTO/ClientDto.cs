using System.ComponentModel.DataAnnotations;
using HBKPlatform.Globals;

namespace HBKPlatform.Models.DTO;

public class ClientDto
{
    public int Id { get; set; }
    
    public Enums.Title Title { get; set; }
    [Required]
    public Enums.Sex Sex { get; set; }
    [Required]
    public string Forename { get; set; }
    [Required]
    public string Surname { get; set; }
    [Required]
    [DataType(DataType.Date)]
    public DateOnly DateOfBirth { get; set; }
    public string? Img { get; set; }
    public string Telephone { get; set; }
    public string Address { get; set; }
    public string? Email { get; set; } // Sourced from User object
    public bool HasUserAccount { get; set; }

    public int ClinicId { get; set; }
}