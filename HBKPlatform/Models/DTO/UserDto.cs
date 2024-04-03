namespace HBKPlatform.Models.DTO
{
    public class UserDto
    {
        public int? PractitionerId { get; set; }
        public int? ClientId { get; set; }

        public int? PracticeId { get; set; }
        public int? ClinicId { get; set; }
        public int TenancyId { get; set; }
        public string UserEmail { get; set; }
        public string UserRole { get; set; }
        public int LoginCount { get; set; }
        public DateTime? LastLogin { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
    }
}