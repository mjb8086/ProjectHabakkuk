using System.ComponentModel.DataAnnotations;
using HBKPlatform.Globals;

namespace HBKPlatform.Database
{
    public class Client : HbkBaseEntity
    {
        public Enums.Title Title { get; set; }
        public Enums.Sex Sex { get; set; }
        public string Forename { get; set; }
        public string Surname { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public string? Img { get; set; }
        public string? UserId { get; set; }
        public string Telephone { get; set; }
        [DataType(DataType.MultilineText)]
        public string? Address { get; set; }
        public int PracticeId { get; set; }
    
        // EF Navigations
        public Practice Practice { get; set; }
        public User User { get; set; }
        public virtual ICollection<ClientMessage> ClientMessages { get; set; }
        public virtual ICollection<ClientPractitioner> ClientPractitioners { get; set; }
    }
}