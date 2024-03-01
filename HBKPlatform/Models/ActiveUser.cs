namespace HBKPlatform.Models;

/* Use in dictionary with userId as key */
public struct ActiveUser
{
    public string UserEmail { get; set; }
    public string UserRole { get; set; }
    public DateTime LastActionTime { get; set; }
    public int TenancyId { get; set; }
    public string Path { get; set; }
}