namespace HBKPlatform.Models.DTO
{
    public class UserDto
    {
        public int? PractitionerId { get; set; }
        public int? ClientId { get; set; }

        public int ClinicId { get; set; }
        public int TenancyId { get; set; }
    }
}